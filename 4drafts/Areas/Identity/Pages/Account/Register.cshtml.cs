using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using _4drafts.Data;
using _4drafts.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using static _4drafts.Data.DataConstants;
using static _4drafts.Services.HtmlHelper;

namespace _4drafts.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [NoDirectAccess]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly _4draftsDbContext data;
        
        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            _4draftsDbContext data)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this.data = data;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(UsernameMaxLength,
            MinimumLength = UsernameMinLength,
            ErrorMessage = "The username must be between 3 and 20 characters long.")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGetAsync(string returnUrl = null)
        {
            return RedirectToAction("Index", "Home");

            //if (this._signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            //ReturnUrl = returnUrl;
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //return Page();
        }

        public IActionResult OnPostAsync(string returnUrl = null)
        {
            return RedirectToAction("Index", "Home");

            //if (this._signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            //returnUrl ??= Url.Content("~/");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //var usernameTaken = this.data.Users.FirstOrDefault(u => u.UserName == Input.Username) != null ? true : false;

            //var emailTaken = this.data.Users.FirstOrDefault(u => u.Email == Input.Email) != null ? true : false;

            //if(usernameTaken)
            //{
            //    this.ModelState.AddModelError(nameof(Input.Username), "Username is already taken.");
            //}

            //if (emailTaken)
            //{
            //    this.ModelState.AddModelError(nameof(Input.Email), "Email address is already taken.");
            //}

            //if (ModelState.IsValid)
            //{
            //    var user = new User { 
            //        UserName = Input.Username, 
            //        Email = Input.Email 
            //    };
            //    var result = await _userManager.CreateAsync(user, Input.Password);
            //    if (result.Succeeded)
            //    {
            //        _logger.LogInformation("User created a new account with password.");

            //        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //        var callbackUrl = Url.Page(
            //            "/Account/ConfirmEmail",
            //            pageHandler: null,
            //            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
            //            protocol: Request.Scheme);

            //        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
            //            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //        if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //        {
            //            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
            //        }
            //        else
            //        {
            //            await _signInManager.SignInAsync(user, isPersistent: false);
            //            return Redirect(returnUrl);
            //        }
            //    }
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return Page();
        }
    }
}
