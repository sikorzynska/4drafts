using _4drafts.Data;
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
        public ThreadsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
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

            var userThreadCount = this.data.Threads
                .Where(t => t.AuthorId == author.Id)
                .Count();

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
                AuthorThreadCount = userThreadCount,
                Points = thread.Points,
                CategoryId = thread.CategoryId,
                Comments = thread.Comments
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    ThreadId = c.ThreadId
                })
                .ToList()
            };

            return View(threadResult);
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

                return View(model);
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

            return Redirect($"/Categories/Browse?categoryId={model.CategoryId}");
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
