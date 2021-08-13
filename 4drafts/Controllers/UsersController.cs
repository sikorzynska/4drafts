﻿using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Threads;
using _4drafts.Models.Users;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Gender = user.Gender,
                Occupation = user.Occupation,
                Website = user.Website,
                Discord = user.Discord,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(userId, this.data),
                CommentCount = UserCommentCount(userId, this.data),
                Threads = this.data.Threads
                .Where(t => t.AuthorId == userId)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Points = t.Points,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                }).ToList(),
            };

            return View(res);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var userId = this.userManager.GetUserId(this.User);

            var user = await this.data.Users
                .Include(u => u.UserThreads)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Redirect("/Identity/Account/Login");

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
                Threads = this.data.Threads
                .Include(t => t.Category)
                .Where(t => t.AuthorId == user.Id)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Points = t.Points,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                }).ToList(),
                LikedThreads = LikedThreads(user, this.data, this.timeWarper),
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

            return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Manage", "Users") });

        }

        //Functions
        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,_4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);

        private static List<ThreadsBrowseModel> LikedThreads(User user, 
            _4draftsDbContext data, 
            ITimeWarper timeWarper)
        {
            var result = new List<ThreadsBrowseModel>();

            foreach (var ut in user.UserThreads)
            {
                var t = data.Threads
                    .Include(t => t.Category)
                    .FirstOrDefault(t => t.Id == ut.ThreadId);

                var thread = new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Points = t.Points,
                    CommentCount = ThreadCommentCount(t.Id, data),
                    CreatedOn = timeWarper.TimeAgo(t.CreatedOn),
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                };

                result.Add(thread);
            }

            return result;
        }
    }
}
