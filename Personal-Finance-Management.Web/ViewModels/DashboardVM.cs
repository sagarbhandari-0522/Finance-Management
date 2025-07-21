using Personal_Finance_Management.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class DashboardVM
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public int CategoriesCount { get; set; }
        public int TransactionCount { get; set; }
        [Required]
        public string TopExpenseCategoryName { get; set; }
        public decimal TopExpenseCategoryAmount { get; set; }
        public decimal MonthlyExpense { get; set; }
        public List<decimal> IncomeExpenseData { get; set; } = new List<decimal> { 2000, 4500 };
        public List<string> CategoriesLabels { get; set; } = null!;
        public List<decimal> CategoriesAmount { get; set; } = null!;
        public List<decimal> MonthAmount { get; set; } = null!;
        public List<Transaction> TransactionList { get; set; } = null!;
    }
}
