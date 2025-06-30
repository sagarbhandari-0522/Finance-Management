using Microsoft.AspNetCore.Mvc.Rendering;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class UpdateTransactionVM
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime? TransactionAt { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }

    }
}
