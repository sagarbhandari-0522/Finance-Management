﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Domain.Enums;
using Personal_Finance_Management.Infrastructure.Repositories;
using Personal_Finance_Management.Web.Helper;
using Personal_Finance_Management.Web.ViewModels;
using System.Collections.Generic;
using System.Text;

namespace Personal_Finance_Management.Web.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(filter: f => (f.UserId == CurrentUserId));
            return View(categories);
        }

        public IActionResult Create()
        {

            var categoryvm = new CreateCategoryVM
            {
                CategoryTypes = CategoryTypeHelper.GetCategory()
            };
            return View(categoryvm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Type = (CategoryType)model.Type,
                    UserId = CurrentUserId

                };
                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            model.CategoryTypes = CategoryTypeHelper.GetCategory();
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(filter: f => (f.Id == id && f.UserId == CurrentUserId));
            if (category == null)
            {
                return RedirectToAction("Error", "Home");

            }
            var updateCategoryVM = new UpdateCategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                CategoryTypes = CategoryTypeHelper.GetCategory()

            };
            return View(updateCategoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryTypes = CategoryTypeHelper.GetCategory();
                return View("Edit", model);

            }
            var category = await _unitOfWork.CategoryRepository.GetAsync(filter: f => (f.Id == model.Id && f.UserId == CurrentUserId));
            if (category == null)
                return RedirectToAction("Error", "Home");
            category.Name = model.Name;
            category.Type = model.Type;
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id, DateOnly? fromDate, DateOnly? toDate)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(include: q => q.Include(c => c.Transactions), filter: f => (f.UserId == CurrentUserId) && f.Id == id);
            if (category == null)
                return RedirectToAction("Error", "Home");
            DateTime from = fromDate?.ToDateTime(TimeOnly.MinValue) ?? new DateTime();
            DateTime to = toDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.UtcNow;
            var transactions = category.Transactions.Where(t => t.CreatedAt >= from && t.CreatedAt <= to).ToList();
            var categoryDetails = new CategoryDetailsVM()
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                TransactionCount = transactions.Count,
                TotalAmount = transactions.Sum(t => t.Amount),
                TransactionList = transactions,
                FromDateFilter = fromDate,
                ToDateFilter = toDate
            };
            return View(categoryDetails);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(filter: f => (f.Id == id && f.UserId == CurrentUserId));
            if (category == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category model)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(filter: f => (f.Id == model.Id && f.UserId == CurrentUserId));
            if (category == null)
            {
                TempData["error"] = "Invalid Request";
                return RedirectToAction("Error", "Home");
            }
            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ExportCSV(DateOnly? fromDate, DateOnly? toDate, int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(filter: f => f.Id == id);
            if (category == null)
                return RedirectToAction("Error", "Home");
            DateTime from = fromDate?.ToDateTime(TimeOnly.MinValue) ?? new DateTime();
            DateTime to = toDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.UtcNow;
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(include: q => q.Include(t => t.Category), filter: f => (f.UserId == CurrentUserId && (f.CreatedAt >= from && f.CreatedAt <= to) && f.Category.Id == id));
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Date,Category,Description,Amount");
            foreach (var transaction in transactions)
            {
                csvBuilder.AppendLine($"{transaction.CreatedAt.ToString("yyyy-MM-dd")},{transaction.Category.Name},{transaction.Description},{transaction.Amount}");
            }
            var fileName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.csv";
            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", $"{fileName}");
        }
    }
}