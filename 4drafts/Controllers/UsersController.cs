using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Users;
using System.Linq;
using System.Threading.Tasks;

namespace PrimalPhobias.Controllers
{

    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly _4draftsDbContext data;

        public UsersController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            _4draftsDbContext data)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.data = data;
        }


        public IActionResult Register() => View();

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (this.data.Users.Any(u => u.UserName == model.Username))
            {
                this.ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
            }
            if (this.data.Users.Any(u => u.Email == model.Email))
            {
                this.ModelState.AddModelError(nameof(model.Email), "Email address is already taken.");
            }
            if (model.Password.Length < 6 || string.IsNullOrWhiteSpace(model.Password))
            {
                this.ModelState.AddModelError(nameof(model.Password), "Password must be at least 6 characters long.");
            }
            if (model.Password != model.PasswordConfirmation)
            {
                this.ModelState.AddModelError(nameof(model.PasswordConfirmation), "Passwords do not match.");
            }

            if (this.ModelState.ErrorCount > 0)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await this.userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            return Redirect("/Users/Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserFormModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password)) return InvalidCredentials(model);

            var loggedInUser = await this.userManager.FindByEmailAsync(model.Email);

            if (loggedInUser == null) return InvalidCredentials(model);

            var passwordIsValid = await this.userManager.CheckPasswordAsync(loggedInUser, model.Password);

            if (!passwordIsValid) return InvalidCredentials(model);

            await this.signInManager.SignInAsync(loggedInUser, true);

            return Redirect("/");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Redirect("/");
        }

        private IActionResult InvalidCredentials(LoginUserFormModel model)
        {
            const string invalidCredentialsMessage = "Credentials invalid. Try again.";

            ModelState.AddModelError(string.Empty, invalidCredentialsMessage);

            return View(model);
        }
    }
}
