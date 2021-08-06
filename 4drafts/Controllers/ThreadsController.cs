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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.HtmlHelper;

namespace _4drafts.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IUserStats userStats;
        private readonly IHtmlHelper htmlHelper;
        private readonly IEntityGetter entityGetter;
        public ThreadsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager,
            IUserStats userStats,
            IHtmlHelper htmlHelper,
            IEntityGetter entityGetter)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
            this.userStats = userStats;
            this.htmlHelper = htmlHelper;
            this.entityGetter = entityGetter;
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
        [NoDirectAccess]
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
                CategoryId = thread.CategoryId,
                Content = thread.Description,
            });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string threadId)
        {
            var thread = await data.Threads.FindAsync(threadId);
            var categoryId = thread.CategoryId;

            var comments = this.data.Comments.Where(c => c.ThreadId == threadId);
            data.RemoveRange(comments);
            data.Threads.Remove(thread);
            await data.SaveChangesAsync();

            var threads = this.entityGetter.CategoryThreads(categoryId, this.data);

            return PartialView("_ThreadsPartial", new CategoryBrowseModel
            {
                Id = categoryId,
                Threads = threads
            });
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public IActionResult Create(int categoryId)
        {
            if (categoryId == 0) return View(new CreateThreadFormModel 
            {
                Categories = this.entityGetter.GetCategories(this.data),
                CategoryName = this.data.Categories.FirstOrDefault().Name,
                CategoryId = this.data.Categories.FirstOrDefault().Id
            });

            return View(new CreateThreadFormModel
            {
                Categories = this.entityGetter.GetCategories(this.data),
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
                model.Categories = this.entityGetter.GetCategories(this.data);
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

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Edit(string threadId)
        {
            var thread = await this.data.Threads.FindAsync(threadId);
            var user = await this.userManager.GetUserAsync(User);

            if (thread == null)
            {
                return NotFound();
            }

            if (user.Id != thread.AuthorId)
            {
                return Unauthorized();
            }

            return View(new EditThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Description,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditThreadViewModel model)
        {
            var thread = await this.data.Threads.FindAsync(model.Id);
            var user = await this.userManager.GetUserAsync(User);

            if (thread == null)
            {
                return NotFound();
            }

            if (user.Id != thread.AuthorId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Edit", model) });
            }

            thread.Title = model.Title;
            thread.Description = model.Content;

            this.data.SaveChanges();

            return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Read", "Threads", new { threadId = thread.Id }) });
        }
    }
}


