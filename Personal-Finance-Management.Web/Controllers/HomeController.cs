using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Domain.Enums;
using Personal_Finance_Management.Infrastructure.Data;
using Personal_Finance_Management.Web.ViewModels;

namespace Personal_Finance_Management.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _logger = logger;
            _context = context;
        }
        [Authorize]

        public IActionResult Index()
        {
            var transactions = _context.Transactions.Where(t => t.UserId == CurrentUserId);
            var totalIncome = transactions.Where(t => t.Category.Type == CategoryType.Income).Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Category.Type == CategoryType.Expense).Sum(t => t.Amount);
            var categoriesCount = transactions.Select(t => t.CategoryId).Distinct().Count();
            var transactionsCount = transactions.Count();
            var topExpenseCategory = transactions.Where(t => t.Category.Type == CategoryType.Expense).GroupBy(t => t.Category).Select(g => new
            {
                Category = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            }).OrderByDescending(x => x.TotalAmount).FirstOrDefault();
            var startDate = DateTime.UtcNow;
            var toDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var monthlyExpense = transactions.Where(t => t.Category.Type == CategoryType.Expense && (t.CreatedAt >= startDate && t.CreatedAt <= toDate)).Sum(t => t.Amount);
            var categoryWithAmountList = transactions.Where(t => t.Category.Type == CategoryType.Expense).GroupBy(t => t.Category.Name).Select(g => new
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            }).ToList();
            var trasactionWIthMonth = transactions.Where(t => t.Category.Type==CategoryType.Expense).GroupBy(t => t.CreatedAt.Month).Select(g => new
            {
                Month = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            }).ToList();
            var Month = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var monthAmount = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (var item in trasactionWIthMonth)
            {
                monthAmount[item.Month-1] = item.TotalAmount;
            }
            var lastFourTransactions = transactions.Include(t => t.Category).OrderByDescending(t =>t.CreatedAt ).Take(4).ToList();

            Console.WriteLine(trasactionWIthMonth);
            var dashboardVM = new DashboardVM()
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                CategoriesCount = categoriesCount,
                TransactionCount = transactionsCount,
                TopExpenseCategoryName = topExpenseCategory?.Category.Name ?? "",
                TopExpenseCategoryAmount = topExpenseCategory?.TotalAmount ?? 0,
                MonthlyExpense = monthlyExpense,
                IncomeExpenseData = new List<decimal> { totalIncome, totalExpense },
                CategoriesLabels = categoryWithAmountList.Select(t => t.CategoryName).ToList(),
                CategoriesAmount = categoryWithAmountList.Select(t => t.TotalAmount).ToList(),
                MonthAmount = monthAmount,
                TransactionList = lastFourTransactions
            };
            return View(dashboardVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
