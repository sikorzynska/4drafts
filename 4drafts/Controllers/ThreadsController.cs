using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Categories;
using _4drafts.Models.Comments;
using _4drafts.Models.Drafts;
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
using System.Security.Claims;
using System.Threading.Tasks;
using static _4drafts.Services.HtmlHelper;

namespace _4drafts.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IHtmlHelper htmlHelper;
        public ThreadsController(ITimeWarper timeWarper,
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

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var liked = this.data
                .UserThreads.Any(ut => ut.UserId == userId && ut.ThreadId == threadId);

            var threadCount = UserThreadCount(thread.AuthorId, this.data);

            var threadResult = new ThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Content,
                Description = thread.Description,
                CreatedOn = this.timeWarper.TimeAgo(thread.CreatedOn),
                AuthorId = author.Id,
                AuthorName = author.UserName,
                AuthorAvatarUrl = author.AvatarUrl,
                AuthorRegisteredOn = author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                AuthorThreadCount = threadCount,
                Points = thread.Points,
                Liked = liked,
                CategoryId = thread.CategoryId,
                Comments = thread.Comments
                .OrderByDescending(t => t.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Points = c.Points,
                    Liked = CommentIsLiked(c.Id, userId, this.data),
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    AuthorCommentCount = UserCommentCount(c.AuthorId, this.data),
                    ThreadId = c.ThreadId
                })
                .ToList()
            };

            return View(threadResult);
        }

        [HttpGet]
        public IActionResult Browse()
        {
            var threads = this.data.Threads
                .Include(t => t.Comments)
                .Include(t => t.Category)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    AuthorAvatarUrl = t.Author.AvatarUrl,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                }).ToList();

            return View(threads);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Library()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Redirect("/");

            var threads = this.data.Threads
                .Include(t => t.Comments)
                .Include(t => t.Category)
                .Where(t => t.AuthorId == user.Id)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    AuthorAvatarUrl = t.Author.AvatarUrl,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                }).ToList();

            return View(threads);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Favourites()
        {
            var userId = this.userManager.GetUserId(this.User);

            var user = await this.data.Users
                .Include(u => u.UserThreads)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Redirect("/");

            var threads = LikedThreads(user, this.data, this.timeWarper);

            return View(threads);
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
            });
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(string threadId)
        {
            var thread = await data.Threads.FindAsync(threadId);

            var comments = this.data.Comments.Where(c => c.ThreadId == threadId);
            data.RemoveRange(comments);
            data.Threads.Remove(thread);
            await data.SaveChangesAsync();

            return Json(new { html = this.htmlHelper.RenderRazorViewToString(this, "DeletedSuccessfully") });
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Create(string title = null, string description = null, string content = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            return View(new CreateThreadFormModel
            {
                Title = title,
                Description = description,
                Content = content,
                Categories = GetCategories(this.data),
                Drafts = this.data.Drafts
                .Where(d => d.AuthorId == user.Id)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    Title = d.Title
                })
                .ToList(),
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateThreadFormModel model)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.data.Categories.Any(c => c.Id == model.CategoryId))
            {
                this.ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = GetCategories(this.data);
                model.Drafts = this.data.Drafts
                .Where(d => d.AuthorId == user.Id)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    Title = d.Title
                })
                .ToList();

                return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Create", model) });
            }

            var thread = new Thread
            {
                Title = model.Title,
                Description = model.Description, 
                Content = model.Content,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = user.Id,
                CategoryId = model.CategoryId,
            };

            await this.data.Threads.AddAsync(thread);
            await this.data.SaveChangesAsync();

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
                Description = thread.Description,
                Content = thread.Content,
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
            thread.Description = model.Description;
            thread.Description = model.Content;

            this.data.SaveChanges();

            return Json(new { isValid = true, redirectToUrl = Url.ActionLink("Read", "Threads", new { threadId = thread.Id }) });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Like(string threadId)
        {
            var thread = await this.data.Threads.FindAsync(threadId);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.data.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (thread == null) return NotFound();

            if (user == null) return Unauthorized();

            var ut = await this.data.UserThreads
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.ThreadId == threadId);

            var liked = false;

            if (ut != null)
            {
                this.data.UserThreads.Remove(ut);
                thread.Points--;
                liked = false;
            }
            else
            {
                this.data.UserThreads.Add(new UserThread
                {
                    UserId = userId,
                    ThreadId = threadId
                });
                thread.Points++;
                liked = true;
            }
            await this.data.SaveChangesAsync();

            var points = this.data.Threads.FirstOrDefault(t => t.Id == threadId).Points;

            return PartialView("_ThreadLikesPartial", new ThreadViewModel
            {
                Id = threadId,
                Points = points,
                Liked = liked,
            });
        }

        //Functions
        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,
                _4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);

        private static IEnumerable<CategoriesBrowseModel> GetCategories(_4draftsDbContext data)
                => data
                  .Categories
                  .Select(c => new CategoriesBrowseModel
                  {
                      Id = c.Id,
                      Name = c.Name
                  })
                  .ToList();
        private static bool CommentIsLiked(string commentId, string userId, _4draftsDbContext data)
                => data.UserComments.Any(uc => uc.UserId == userId && uc.CommentId == commentId);

        private static List<ThreadsBrowseModel> LikedThreads(User user,
                _4draftsDbContext data,
                ITimeWarper timeWarper)
                    {
                        var result = new List<ThreadsBrowseModel>();
               
                        foreach (var ut in user.UserThreads)
                        {
                            var t = data.Threads
                                .Include(t => t.Author)
                                .Include(t => t.Category)
                                .FirstOrDefault(t => t.Id == ut.ThreadId);
               
                            var thread = new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                CategoryId = t.CategoryId,
                                CategoryName = t.Category.Name,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                AuthorName = t.Author.UserName,
                                CommentCount = ThreadCommentCount(t.Id, data),
                                CreatedOn = timeWarper.TimeAgo(t.CreatedOn),
                            };
               
                            result.Add(thread);
                        }
               
                        return result;
                    }
                }
}


