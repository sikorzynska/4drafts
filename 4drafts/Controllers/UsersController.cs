using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Threads;
using _4drafts.Models.Users;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.HtmlHelper;

namespace _4drafts.Controllers
{
    public class UsersController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IHtmlHelper htmlHelper;
        public UsersController(_4draftsDbContext data,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHtmlHelper htmlHelper)
        {
            this.data = data;
            this.userManager = userManager;
            this.htmlHelper = htmlHelper;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        [NoDirectAccess]
        public IActionResult Register(string returnUrl = null)
        {
            if (this.signInManager.IsSignedIn(this.User)) return RedirectToAction("Index", "Home");

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
            if (this.signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            var usernameTaken = this.data.Users.FirstOrDefault(u => u.UserName == model.Username) != null ? true : false;

            var emailTaken = this.data.Users.FirstOrDefault(u => u.Email == model.Email) != null ? true : false;

            if (usernameTaken) this.ModelState.AddModelError(nameof(model.Username), "Username is already taken.");

            if (emailTaken) this.ModelState.AddModelError(nameof(model.Email), "Email address is already taken.");

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
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

            return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Register", model) });
        }

        [HttpGet]
        [AllowAnonymous]
        [NoDirectAccess]
        public IActionResult Login(string returnUrl = null)
        {
            if (this.signInManager.IsSignedIn(this.User)) return RedirectToAction("Index", "Home");

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
            if (this.signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            var appliedError = false;

            const string invalidCredentials = "Whoops! Invalid credentials.";

            var existingUser = await this.userManager.FindByEmailAsync(model.Email);

            if (existingUser == null)
            {
                ModelState.AddModelError(string.Empty, invalidCredentials);
                appliedError = true;
            }

            var passwordIsValid = await this.userManager.CheckPasswordAsync(existingUser, model.Password);

            if (!passwordIsValid && !appliedError) ModelState.AddModelError(string.Empty, invalidCredentials);

            if (!ModelState.IsValid) return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Login", model) });

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

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> Profile(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                ThreadCount = UserThreadCount(userId, this.data),
                CommentCount = UserCommentCount(userId, this.data),

            };

            return View(res);
        }

        [HttpGet]
        public async Task<IActionResult> Browse(string userId)
        {
            var user = await this.data.Users
                .Include(u => u.UserThreads)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Gender = user.Gender,
                Age = user.Age,
                Occupation = user.Occupation,
                Website = user.Website,
                Discord = user.Discord,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),
            };

            return View(res);
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Edit()
        {
            var user = await this.userManager.GetUserAsync(User);

            return View(new UserViewModel
            {
                Id = user.Id,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Occupation = user.Occupation,
                AboutMe = user.AboutMe,
                Website = user.Website,
                Facebook = user.Facebook,
                Discord = user.Discord,
                Instagram = user.Instagram,
                Twitter = user.Twitter,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.Gender))
            {
                if (model.Gender != "Male" && model.Gender != "Female")
                this.ModelState.AddModelError(nameof(model.Gender), "Invalid gender selection");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Edit", model) });
            }

            var userId = this.userManager.GetUserId(this.User);

            var user = await this.data.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            user.AvatarUrl = model.AvatarUrl;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.Age = model.Age;
            user.Occupation = model.Occupation;
            user.AboutMe = model.AboutMe;
            user.Website = model.Website;
            user.Discord = model.Discord;
            user.Facebook = model.Facebook;
            user.Twitter = model.Twitter;
            user.Instagram = model.Instagram;

            await this.data.SaveChangesAsync();

            return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Browse", "Users", new { userId = user.Id }) });

        }

        //Functions
        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,_4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);
    }
}
