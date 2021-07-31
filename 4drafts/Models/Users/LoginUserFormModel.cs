using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Users
{
    public class LoginUserFormModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
