using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Threads;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace _4drafts.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;

        public CommentsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(ThreadViewModel model)
        {
            var thread = this.data.Threads.FirstOrDefault(t => t.Id == model.Id);

            if(string.IsNullOrWhiteSpace(model.Id) || thread == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.CommentContent))
            {
                this.ModelState.AddModelError(nameof(model.CommentContent), "Comments cannot be empty...");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var comment = new Comment
            {
                Content = model.CommentContent,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = this.userManager.GetUserId(User),
                ThreadId = model.Id
            };

            this.data.Comments.Add(comment);
            this.data.SaveChanges();

            return RedirectToAction("Read", "Threads", new { threadId = model.Id });
        }
    }
}
