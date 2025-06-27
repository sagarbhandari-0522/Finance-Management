using Microsoft.AspNetCore.Mvc.Rendering;
using Personal_Finance_Management.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class UpdateCategoryVM
    {
        public required int Id { get; set; }
        [Required]
        public  string Name { get; set; }
        [Required]
        public  CategoryType Type { get; set; }
        public List<SelectListItem>? CategoryTypes { get; set; }
    }
}
