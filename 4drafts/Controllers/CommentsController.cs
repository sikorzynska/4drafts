﻿using _4drafts.Data;
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
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using static _4drafts.Services.ControllerExtensions;
using _4drafts.Models.Shared;
using static _4drafts.Data.DataConstants;

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
        public async Task<IActionResult> Create(string threadId, string Content)
        {
            var thread = await this.data.Threads.FindAsync(threadId);
            var user = await this.userManager.GetUserAsync(User);

            var characterCount = string.IsNullOrWhiteSpace(Content) ? 0 : Content.Length;

            if (user == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(threadId) || thread == null) return BadRequest();

            if (string.IsNullOrWhiteSpace(Content))
            {
                this.ModelState.AddModelError(nameof(Content), EmptyComment);
            }

            if(characterCount > 500)
            {
                this.ModelState.AddModelError(nameof(Content), MaxLengthComment);
            }

            if (!ModelState.IsValid) return BadRequest();

            var comment = new Comment
            {
                Content = Content,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = user.Id,
                ThreadId = threadId
            };

            this.data.Comments.Add(comment);
            this.data.SaveChanges();

            var comments = this.data.Comments
                .Where(c => c.ThreadId == threadId)
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Points = c.Points,
                    Liked = IsLiked(c.Id, user.Id, this.data),
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    AuthorCommentCount = UserCommentCount(c.AuthorId, this.data),
                    ThreadId = c.ThreadId
                })
                .ToList();

            return PartialView("_CommentsPartial", new ThreadViewModel
            {
                Id = threadId,
                Comments = comments
            });
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Edit(string Id)
        {
            var comment = await this.data.Comments.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(User);

            if (comment == null) return Json(new { isValid = false, msg = InexistentComment });

            if (user == null || user.Id != comment.AuthorId) return Json(new { isValid = false, msg = UnauthorizedAction });

            return Json(new
            {
                isValid = true,
                html = RenderRazorViewToString(this, "Edit", 
                new EditCommentViewModel 
                { 
                    Id = comment.Id, 
                    Content = comment.Content 
                })
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditCommentViewModel model)
        {
            var comment = await this.data.Comments.FindAsync(model.Id);
            var user = await this.userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return Json(new { isValid = false, html = RenderRazorViewToString(this, "Edit", model) });
            }

            comment.Content = model.Content;
            this.data.SaveChanges();

            var comments = this.data.Comments
                .Where(c => c.ThreadId == comment.ThreadId)
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Points = c.Points,
                    Liked = IsLiked(c.Id, user.Id, this.data),
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    AuthorCommentCount = UserCommentCount(c.AuthorId, this.data),
                    ThreadId = c.ThreadId
                })
                .ToList();

            return Json(new
            {
                isValid = true,
                msg = SuccessfullyUpdatedComment,
                entity = "comment",
                html = RenderRazorViewToString(this, "_CommentsPartial", new ThreadViewModel { Id = comment.ThreadId, Comments = comments })
            });
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(string Id)
        {
            var comment = await this.data.Comments.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(User);

            if (comment == null) return Json(new { isValid = false, msg = InexistentComment });

            if (user == null || user.Id != comment.AuthorId) return Json(new { isValid = false, msg = UnauthorizedAction });

            return Json(new { isValid = true, html = RenderRazorViewToString(this, "DeleteEntity", new GlobalViewModel { Id = comment.Id, Name = "comment", Path = "/Comments/Delete/" }) });
        }

        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(string Id)
        {
            var comment = await this.data.Comments.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(User);

            if (comment == null) return Json(new { isValid = false, 
                msg = InexistentComment });

            if (user == null || user.Id != comment.AuthorId || user == null) return Json(new { isValid = false, 
                msg = UnauthorizedAction });

            var thread = await this.data.Threads.FindAsync(comment.ThreadId);

            this.data.Comments.Remove(comment);
            await this.data.SaveChangesAsync();

            var comments = this.data.Comments
                .Where(c => c.ThreadId == thread.Id)
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Points = c.Points,
                    Liked = IsLiked(c.Id, user.Id, this.data),
                    CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.UserName,
                    AuthorAvatarUrl = c.Author.AvatarUrl,
                    AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                    AuthorCommentCount = UserCommentCount(c.AuthorId, this.data),
                    ThreadId = c.ThreadId
                })
                .ToList();

            return Json(new { isValid = true, 
                msg = SuccessfullyDeletedComment,
                html = RenderRazorViewToString(this, "_CommentsPartial", 
                new ThreadViewModel { Id = thread.Id, Comments = comments }), 
                entity = "comment" });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Like(string commentId)
        {
            var comment = await this.data.Comments.FindAsync(commentId);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await this.data.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (comment == null) return NotFound();

            if (user == null) return Unauthorized();

            var uc = await this.data.UserComments
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CommentId == commentId);

            if (uc != null)
            {
                this.data.UserComments.Remove(uc);
                comment.Points--;
            }
            else
            {
                this.data.UserComments.Add(new UserComment
                {
                    UserId = userId,
                    CommentId = commentId
                });
                comment.Points++;
            }
            await this.data.SaveChangesAsync();

            var comments = this.data.Comments
                  .Where(c => c.ThreadId == comment.ThreadId)
                  .OrderByDescending(c => c.CreatedOn)
                  .Select(c => new CommentViewModel
                  {
                      Id = c.Id,
                      Content = c.Content,
                      Points = c.Points,
                      Liked = IsLiked(c.Id, userId, this.data),
                      CreatedOn = timeWarper.TimeAgo(c.CreatedOn),
                      AuthorId = c.AuthorId,
                      AuthorName = c.Author.UserName,
                      AuthorAvatarUrl = c.Author.AvatarUrl,
                      AuthorRegisteredOn = c.Author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                      AuthorCommentCount = UserCommentCount(c.AuthorId, this.data),
                      ThreadId = c.ThreadId
                  })
                  .ToList();

            return PartialView("_CommentsPartial", new ThreadViewModel
            {
                Id = comment.ThreadId,
                Comments = comments,
            });
        }

        private static int UserCommentCount(string userId, _4draftsDbContext data)
               => data.Comments.Count(c => c.AuthorId == userId);

        private static bool IsLiked(string commentId, string userId, _4draftsDbContext data)
            => data.UserComments.Any(uc => uc.UserId == userId && uc.CommentId == commentId);
    }
}
