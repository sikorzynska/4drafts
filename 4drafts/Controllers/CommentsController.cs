using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Threads;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using _4drafts.Models.Comments;
using System.Globalization;
using System.Threading.Tasks;

namespace _4drafts.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IUserStats userStats;
        private readonly IHtmlHelper htmlHelper;

        public CommentsController(ITimeWarper timeWarper,
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

        [HttpPost]
        [Authorize]
        public IActionResult Create(string Id, string Content)
        {
            var thread = this.data.Threads.FirstOrDefault(t => t.Id == Id);

            var characterCount = Content.Length;

            if (string.IsNullOrWhiteSpace(Id) || thread == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(Content) || characterCount > 500)
            {
                this.ModelState.AddModelError(nameof(Content), "Comments cannot be empty or longer than 500 characters");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var comment = new Comment
            {
                Content = Content,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = this.userManager.GetUserId(User),
                ThreadId = Id
            };

            this.data.Comments.Add(comment);
            this.data.SaveChanges();

            var comments = this.data.Comments
                .Where(c => c.ThreadId == Id)
                .OrderByDescending(c => c.CreatedOn)
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
                .ToList();

            return PartialView("_CommentsPartial", new ThreadViewModel
            {
                Id = Id,
                Comments = comments
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string commentId)
        {
            var comment = await this.data.Comments.FindAsync(commentId);
            var user = await this.userManager.GetUserAsync(User);

            if (comment == null)
            {
                return NotFound();
            }

            if (user.Id != comment.AuthorId)
            {
                return Unauthorized();
            }

            return View(new EditCommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditCommentViewModel model)
        {
            var comment = await this.data.Comments.FindAsync(model.Id);
            var user = await this.userManager.GetUserAsync(User);

            if (comment == null)
            {
                return NotFound();
            }

            if (user.Id != comment.AuthorId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json(new { isValid = false, html = htmlHelper.RenderRazorViewToString(this, "Edit", model) });
            }

            var thread = await this.data.Threads.FindAsync(comment.ThreadId);

            //Update database entity & save changes
            comment.Content = model.Content;
            this.data.SaveChanges();

            var comments = this.data.Comments
                .Where(c => c.ThreadId == thread.Id)
                .OrderByDescending(c => c.CreatedOn)
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
                .ToList();

            return Json(new { isValid = true, html = htmlHelper.RenderRazorViewToString(this, "_CommentsPartial", new ThreadViewModel { Id = thread.Id, Comments = comments }) });
        }
    }
}
