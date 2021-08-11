using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Users;
using _4drafts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.HtmlHelper;

namespace _4drafts.Controllers
{
    public class UsersController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IHtmlHelper htmlHelper;
        public UsersController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager,
            IHtmlHelper htmlHelper)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
            this.htmlHelper = htmlHelper;
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
                FirstName = user.FirstName == null ? "---" : user.FirstName,
                LastName = user.LastName == null ? "---" : user.LastName,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Gender = user.Gender == null ? "---" : user.Gender,
                Website = user.Website == null ? "---" : user.Website,
                Github = user.Github == null ? "---" : user.Github,
                Twitter = user.Twitter == null ? "---" : user.Twitter,
                Facebook = user.Facebook == null ? "---" : user.Facebook,
                Instagram = user.Instagram == null ? "---" : user.Instagram,
                AboutMe = user.AboutMe == null ? "---" : user.AboutMe,
                ThreadCount = UserThreadCount(userId, this.data),
                CommentCount = UserCommentCount(userId, this.data),
            };

            return View(res);
        }

        //Functions
        private static int UserThreadCount(string userId, _4draftsDbContext data)
        => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);
    }
}
