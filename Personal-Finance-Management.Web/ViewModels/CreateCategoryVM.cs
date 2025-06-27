using Microsoft.AspNetCore.Mvc.Rendering;
using Personal_Finance_Management.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class CreateCategoryVM
    {
        [Required]
        public  string Name { get; set; }
        [Required(ErrorMessage = "Please select a category type.")]
        public CategoryType? Type { get; set; }
        public List<SelectListItem> CategoryTypes { get; set; } = new();

    }
}
