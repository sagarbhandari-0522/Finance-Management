using Microsoft.AspNetCore.Mvc;
using Personal_Finance_Management.Application.Interfaces.IRepositories;
using Personal_Finance_Management.Web.Helper;
using Personal_Finance_Management.Web.ViewModels;
using Personal_Finance_Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Personal_Finance_Management.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync(include: q =>q.Include(t => t.Category));
            return View(transactions);
        }

        public async Task<IActionResult> Create()
        {
            var transactionVM = new CreateTransactionVM
            {
                CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync())
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
                    CreatedAt = model.TransactionAt ?? DateTime.UtcNow
                };
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            model.CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync());
            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(
                filter: t => t.Id==id,
                include: q => q.Include(t=> t.Category)
                );
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            return View(transaction);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.FindAsync(id);
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            var updateTransactionVM = new UpdateTransactionVM
            {
                Amount = transaction.Amount,
                Description = transaction.Description,
                CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync()),
                CategoryId = transaction.CategoryId,
                TransactionAt=transaction.CreatedAt
            };
            return View(updateTransactionVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateTransactionVM model)
        {
            if(!ModelState.IsValid)
            {
                model.CategoryList = CategoryListHelper.CategoryList(await _unitOfWork.CategoryRepository.GetAllAsync());
                return View("Edit", model);
            }
            var transaction = await _unitOfWork.TransactionRepository.FindAsync(model.Id);
            if (transaction == null)
                return RedirectToAction("Error", "Home");
            transaction.Description = model.Description;
            transaction.Amount = model.Amount;
            transaction.CategoryId = model.CategoryId;
            transaction.CreatedAt = model.TransactionAt ?? DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(
               filter: t => t.Id ==id,
               include: q => q.Include(t=> t.Category));
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
            return RedirectToAction("Index");
        }

    }
}
