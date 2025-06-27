using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Infrastructure.Repositories;
using Personal_Finance_Management.Web.ViewModels;

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
            return View();
        }
        public async Task<IActionResult> Create (CreateCategoryVM model)
        {
            if(ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Type = model.Type,

                };
                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
                Id=category.Id,
                Name = category.Name,
                Type=category.Type
            };
            return View(updateCategoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
                return View("Edit");
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category model)
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(model);
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
