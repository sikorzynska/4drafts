using _4drafts.Data.Models;
using _4drafts.Models.Authentication;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static _4drafts.Data.DataConstants;
using static _4drafts.Services.ControllerExtensions;

namespace _4drafts.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IAuthenticator auth;
        public AuthenticationController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthenticator auth)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.auth = auth;
        }

        [HttpGet]
        [AllowAnonymous]
        [NoDirectAccess]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (currentUser != null) return RedirectToAction("Index", "Home");

            return View(new UserRegisterFormModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [NoDirectAccess]
        public async Task<IActionResult> Register(UserRegisterFormModel model)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (currentUser != null) return RedirectToAction("Index", "Home");

            if (auth.UsernameTaken(model.Username)) this.ModelState.AddModelError(nameof(model.Username), Users.UsernameTaken);

            if (auth.EmailTaken(model.Email)) this.ModelState.AddModelError(nameof(model.Email), Users.EmailTaken);

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username.Replace(" ", "_"),
                    Email = model.Email
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var returnUrl = model.ReturnUrl == null ? Url.Content("/") : Url.Content(model.ReturnUrl);
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return Json(new { isValid = true, redirectUrl = Url.Content(returnUrl) });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Json(new { isValid = false, html = RenderRazorViewToString(this, "Register", model) });
        }

        [HttpGet]
        [AllowAnonymous]
        [NoDirectAccess]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (currentUser != null) return RedirectToAction("Index", "Home");

            return View(new UserLoginFormModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [NoDirectAccess]
        public async Task<IActionResult> Login(UserLoginFormModel model)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (currentUser != null) return RedirectToAction("Index", "Home");

            var appliedError = false;

            User existingUser = null;

            if (!this.auth.EmailExists(model.Email))
            {
                ModelState.AddModelError(string.Empty, Users.InvalidCredentials);
                appliedError = true;
            }

            if(!appliedError) existingUser = await this.userManager.FindByEmailAsync(model.Email);

            var passwordIsValid = await this.userManager.CheckPasswordAsync(existingUser, model.Password);
             
            if (!passwordIsValid && !appliedError) ModelState.AddModelError(string.Empty, Users.InvalidCredentials);

            if (!ModelState.IsValid) return Json(new { isValid = false, html = RenderRazorViewToString(this, "Login", model) });

            var returnUrl = model.ReturnUrl == null ? Url.Content("/") : Url.Content(model.ReturnUrl);

            await this.signInManager.SignInAsync(existingUser, true);
            return Json(new { isValid = true, redirectUrl = Url.Content(returnUrl) });
        }

        [HttpGet]
        [NoDirectAccess]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            var ReturnUrl = returnUrl == null ? Url.Content("/") : returnUrl;
            await signInManager.SignOutAsync();
            return Redirect(ReturnUrl);
        }
    }
}
