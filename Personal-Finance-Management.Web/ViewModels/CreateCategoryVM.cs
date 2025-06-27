using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class CreateCategoryVM
    {
        [Required]
        public  string Name { get; set; }
        [Required]
        public   string Type { get; set; }

    }
}
