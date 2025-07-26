using Microsoft.AspNetCore.Mvc;
using System;
using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Web.Helper;
using Personal_Finance_Management.Web.ViewModels;
using Personal_Finance_Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Personal_Finance_Management.Web.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(DateOnly? fromDate, DateOnly? toDate)
        {
            DateTime fromDateTime = fromDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
            DateTime toDateTime = toDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.UtcNow;
            Console.WriteLine($"{fromDateTime}+{toDateTime}");
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(include: q => q.Include(t => t.Category), filter: f => (f.UserId == CurrentUserId && (f.CreatedAt >= fromDateTime && f.CreatedAt <= toDateTime)));
            var transactionListVM = new TransactionListVM()
            {
                Transactions = transactions,
                ToDate = toDate,
                FromDate = fromDate
            };
            return View(transactionListVM);
        }

        public async Task<IActionResult> Create()
        {
            var transactionVM = new CreateTransactionVM
            {
                CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync(filter: f => (f.UserId == CurrentUserId)))
            };
            return View(transactionVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionVM model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    Description = model.Description,
                    Amount = model.Amount,
                    CategoryId = model.CategoryId,
                    CreatedAt = model.TransactionAt ?? DateTime.UtcNow,
                    UserId = CurrentUserId
                };
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                TempData["success"] = "Transaction added successfully";
                return RedirectToAction("Index");
            }
            model.CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync());
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(
                filter: f => (f.Id == id && f.UserId == CurrentUserId),
                include: q => q.Include(t => t.Category)
                );
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            return View(transaction);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(filter: f => (f.Id == id && f.UserId == CurrentUserId));
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            var updateTransactionVM = new UpdateTransactionVM
            {
                Amount = transaction.Amount,
                Description = transaction.Description,
                CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync()),
                CategoryId = transaction.CategoryId,
                TransactionAt = transaction.CreatedAt
            };
            return View(updateTransactionVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateTransactionVM model)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync());
                return View("Edit", model);
            }
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(filter: f => (f.Id == model.Id && f.UserId == CurrentUserId));
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            transaction.Description = model.Description;
            transaction.Amount = model.Amount;
            transaction.CategoryId = model.CategoryId;
            transaction.CreatedAt = model.TransactionAt ?? DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            TempData["success"] = "Transaction updated successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(
               filter: f => (f.Id == id && f.UserId == CurrentUserId),
               include: q => q.Include(t => t.Category));
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            return View(transaction);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Transaction model)
        {
            if (model == null)
                return RedirectToAction("Error", "Home");
            _unitOfWork.TransactionRepository.Remove(model);
            await _unitOfWork.SaveChangesAsync();
            TempData["success"] = "Transaction Deleted";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportCSV(DateOnly? fromDate, DateOnly? toDate)
        {
            DateTime fromDateTime = fromDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
            DateTime toDateTime = toDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.UtcNow;
            Console.WriteLine($"{fromDateTime}+{toDateTime}");
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(include: q => q.Include(t => t.Category), filter: f => ((f.UserId == CurrentUserId) && (f.CreatedAt >= fromDateTime && f.CreatedAt <= toDateTime)));
            Console.WriteLine(transactions.Count);
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Date,Category,Description,Amount");
            foreach (var transaction in transactions)
            {
                csvBuilder.AppendLine($"{transaction.CreatedAt.ToString("yyyy-MM-dd")},{transaction.Category.Name},{transaction.Description},{transaction.Amount}");
            }
            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.csv");
        }
    }
}
