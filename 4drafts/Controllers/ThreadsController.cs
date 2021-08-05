﻿using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Categories;
using _4drafts.Models.Comments;
using _4drafts.Models.Threads;
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

namespace _4drafts.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IUserStats userStats;
        private readonly IHtmlHelper htmlHelper;
        public ThreadsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager,
            IUserStats userStats,
            IHtmlHelper htmlHelper)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
            this.userStats = userStats;
            this.htmlHelper = htmlHelper;
        }

        public async Task<IActionResult> Read(string threadId)
        {
            var thread = this.data.Threads
                .Include(t => t.Author)
                .Include(t => t.Category)
                .Include("Comments.Author")
                .FirstOrDefault(t => t.Id == threadId);

            if (thread == null) return NotFound();

            var author = await this.userManager.FindByIdAsync(thread.AuthorId);

            if (author == null) return NotFound();

            var threadCount = this.userStats.userThreadCount(thread.AuthorId, this.data);

            var threadResult = new ThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Description,
                CreatedOn = this.timeWarper.TimeAgo(thread.CreatedOn),
                AuthorId = author.Id,
                AuthorName = author.UserName,
                AuthorAvatarUrl = author.AvatarUrl,
                AuthorRegisteredOn = author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                AuthorThreadCount = threadCount,
                Points = thread.Points,
                CategoryId = thread.CategoryId,
                Comments = thread.Comments
                .OrderByDescending(t => t.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    AuthorCommentCount = this.userStats.userCommentCount(c.AuthorId, this.data),
                    ThreadId = c.ThreadId
                })
                .ToList()
            };

            return View(threadResult);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(string threadId)
        {
            var thread = await data.Threads.FindAsync(threadId);
            var user = await this.userManager.GetUserAsync(User);

            if (thread == null)
            {
                return NotFound();
            }

            if(user.Id != thread.AuthorId)
            {
                return Unauthorized();
            }

            return View(new ThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Description,
            });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string threadId)
        {
            var thread = await data.Threads.FindAsync(threadId);
            var categoryId = thread.CategoryId;

            data.Threads.Remove(thread);
            await data.SaveChangesAsync();

            return RedirectToAction("Browse", "Categories", new { categoryId = categoryId });
            //return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Browse", "Categories", new { categoryId = categoryId }) });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int categoryId)
        {
            if (categoryId == 0) return View(new CreateThreadFormModel 
            {
                Categories = this.GetCategories(),
                CategoryName = this.data.Categories.FirstOrDefault().Name,
                CategoryId = this.data.Categories.FirstOrDefault().Id
            });

            return View(new CreateThreadFormModel
            {
                Categories = this.GetCategories(),
                CategoryName = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Name,
                CategoryId = categoryId
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateThreadFormModel model)
        {
            if (!this.data.Categories.Any(c => c.Id == model.CategoryId))
            {
                this.ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = this.GetCategories();
                model.CategoryName = model.Categories.FirstOrDefault(c => c.Id == model.CategoryId).Name;

                return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Create", model) });
            }

            var thread = new Thread
            {
                Title = model.Title,
                Description = model.Content,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = this.userManager.GetUserId(this.User),
                CategoryId = model.CategoryId,
            };

            this.data.Threads.Add(thread);
            this.data.SaveChanges();

            return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Read", "Threads", new { threadId = thread.Id }) });
        }


        private IEnumerable<CategoriesBrowseModel> GetCategories()
            => this.data
                .Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
    }
}


