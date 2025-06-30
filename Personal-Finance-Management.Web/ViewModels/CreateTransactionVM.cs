using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class CreateTransactionVM
    {
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime? TransactionAt { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }
    }
}
