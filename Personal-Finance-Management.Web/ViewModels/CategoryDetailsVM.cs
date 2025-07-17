using System.ComponentModel.DataAnnotations;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Domain.Enums;
namespace Personal_Finance_Management.Web.ViewModels
{
    public class CategoryDetailsVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public CategoryType Type { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Transaction> TransactionList { get; set; } = new List<Transaction>();
    }
}
