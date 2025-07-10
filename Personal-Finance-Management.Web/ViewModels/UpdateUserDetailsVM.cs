using System.ComponentModel.DataAnnotations;

namespace Personal_Finance_Management.Web.ViewModels
{
    public class UpdateUserDetailsVM
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserId { get; set; }
        public required bool IsExternalUser { get; set; }

    }
}
