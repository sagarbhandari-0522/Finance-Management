using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class UserRegisterVM
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not Match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
