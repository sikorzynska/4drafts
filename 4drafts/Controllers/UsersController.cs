﻿using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.ControllerExtensions;
using static _4drafts.Data.DataConstants;
using _4drafts.Services;

namespace _4drafts.Controllers
{
    public class UsersController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        public UsersController(_4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> Peek(string u)
        {
            var user = await this.userManager.FindByNameAsync(u);

            if (user == null) return Json(new { isValid = false, msg = Global.GeneralError });

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email,
                Points = user.Points,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),

            };

            return Json(new { isValid = true, html = RenderRazorViewToString(this, "Peek", res) });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Redirect("/");

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
                Youtube = user.Youtube,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Patreon = user.Patreon,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),
            };

            return View(res);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Manage(UserViewModel model)
        {
            if (model.Gender != "Male" && model.Gender != "Female" && model.Gender != string.Empty && model.Gender != null)
                this.ModelState.AddModelError(nameof(model.Gender), Users.InvalidGender);

            if (!ModelState.IsValid) return View(model);

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
            user.Youtube = model.Youtube;
            user.Facebook = model.Facebook;
            user.Twitter = model.Twitter;
            user.Instagram = model.Instagram;
            user.Patreon = model.Patreon;

            await this.data.SaveChangesAsync();

            return Redirect($"/Users/Profile?u={user.UserName}");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string u)
        {
            var user = await this.data.Users
                .Include(x => x.UserThreads)
                .FirstOrDefaultAsync(x => x.UserName == u);

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
                Points = user.Points,
                Occupation = user.Occupation,
                Website = user.Website,
                Youtube = user.Youtube,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Patreon = user.Patreon,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),
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
