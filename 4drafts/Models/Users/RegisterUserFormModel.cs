using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Models.Users
{
    public class RegisterUserFormModel
    {
        [Required]
        [StringLength(UsernameMaxLength,
        MinimumLength = UsernameMinLength,
        ErrorMessage = "The username must be between 3 and 20 characters long.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(PasswordMinLength,
        ErrorMessage = "The password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Password confirmation")]
        public string PasswordConfirmation { get; set; }
    }
}
