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

namespace _4drafts.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly IUserStats userStats;

        public CommentsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager,
            IUserStats userStats)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
            this.userStats = userStats;
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
    }
}
