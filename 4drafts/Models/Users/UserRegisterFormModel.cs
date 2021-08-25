using _4drafts.Data;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Users
{
    public class UserRegisterFormModel
    {
        [Required]
        [StringLength(DataConstants.Users.UsernameMaxLength,
        MinimumLength = DataConstants.Users.UsernameMinLength,
        ErrorMessage = DataConstants.Users.UsernameLengthMsg)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(DataConstants.Users.PasswordMaxLength,
            MinimumLength = DataConstants.Users.PasswordMinLength,
            ErrorMessage = DataConstants.Users.PasswordLengthMsg)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = DataConstants.Users.PasswordConfirmMsg)]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
