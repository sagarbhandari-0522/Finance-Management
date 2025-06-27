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

namespace Personal_Finance_Management.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
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
            var category =await  _unitOfWork.CategoryRepository.FindAsync(id);
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
                return View("Edit",model);

            }
            var category = await _unitOfWork.CategoryRepository.FindAsync(model.Id);
            if (category == null)
                return RedirectToAction("Error", "Home");
            category.Name = model.Name;
            category.Type = model.Type;
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(id);
            if(category==null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category model)
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(model.Id);
            if(category==null)
            {
                TempData["error"] = "Invalid Request";
                return RedirectToAction("Error", "Home");
            }
            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
