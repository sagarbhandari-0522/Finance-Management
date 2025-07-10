using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class UpdatePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage="Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string UserId { get; set; }
        public required bool IsExternalUser { get; set; }

    }
}
