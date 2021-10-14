using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Genres;
using _4drafts.Models.Comments;
using _4drafts.Models.Drafts;
using _4drafts.Models.Shared;
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
using static _4drafts.Services.ControllerExtensions;
using static _4drafts.Data.DataConstants;

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

        [HttpGet]
        public async Task<IActionResult> Read(string t)
        {
            var thread = this.data.Threads
                .Include(x => x.Author)
                .Include(x => x.GenreThreads)
                .Include(x => x.ThreadType)
                .Include("Comments.Author")
                .FirstOrDefault(x => x.Id == t);

            if (thread == null) return NotFound();

            var author = await this.userManager.FindByIdAsync(thread.AuthorId);

            if (author == null) return NotFound();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var liked = this.data
                .UserThreads.Any(ut => ut.UserId == userId && ut.ThreadId == t);

            var threadCount = UserThreadCount(thread.AuthorId, this.data);

            var threadResult = new ThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                TypeId = thread.ThreadTypeId,
                Type = thread.ThreadType.Name,
                Content = thread.Content,
                CreatedOn = this.timeWarper.TimeAgo(thread.CreatedOn),
                AuthorId = author.Id,
                AuthorName = author.UserName,
                AuthorAvatarUrl = author.AvatarUrl,
                AuthorRegisteredOn = author.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                AuthorThreadCount = threadCount,
                Points = thread.Points,
                Liked = liked,
                GenreIds = thread.GenreThreads.Select(gt => gt.GenreId),
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
        public async Task<IActionResult> Browse(int genre = 0, int type = 0, string sort = "best", int page = 1, string u = null, bool liked = false)
        {
            var threads = new List<ThreadsBrowseModel>();

            var user = await this.userManager.GetUserAsync(this.User);

            if (genre == 0 && type == 0)
            {
                switch (sort)
                {
                    case "controversial":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "best":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "new":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                Content = t.Content,
                                ThreadTypeName = t.ThreadType.Name,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    default:
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                }
            }
            else if(genre != 0 && type == 0)
            {
                switch (sort)
                {
                    case "controversial":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "best":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "new":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    default:
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                }
            }
            else if(genre == 0 && type != 0)
            {
                switch (sort)
                {
                    case "controversial":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.ThreadTypeId == type)
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "best":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.ThreadTypeId == type)
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "new":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.ThreadTypeId == type)
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    default:
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.ThreadTypeId == type)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                }
            }
            else
            {
                switch (sort)
                {
                    case "controversial":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre) && t.ThreadTypeId == type)
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "best":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre) && t.ThreadTypeId == type)
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    case "new":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre) && t.ThreadTypeId == type)
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                    default:
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .Include(t => t.ThreadType)
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre) && t.ThreadTypeId == type)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                ThreadTypeId = t.ThreadTypeId,
                                ThreadTypeName = t.ThreadType.Name,
                                Content = t.Content,
                                Genres = GetGenres(this.data, 0, t.Id),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                                FullDate = this.timeWarper.FullDate(t.CreatedOn),
                                Points = t.Points,
                                AuthorId = t.AuthorId,
                                AuthorName = t.Author.UserName,
                                AuthorAvatarUrl = t.Author.AvatarUrl,
                                CommentCount = ThreadCommentCount(t.Id, this.data),
                            }).ToList();
                            break;
                        }
                }
            }

            if (u != null) threads = threads.Where(t => t.AuthorName == u).ToList();

            if (liked) threads = threads.Where(t => this.data.UserThreads.Any(ut => ut.ThreadId == t.Id && ut.UserId == user.Id)).ToList();

            if(user != null) 
                foreach (var thread in threads) 
                    if (ThreadIsLiked(thread.Id, user.Id, this.data)) thread.Liked = true;

            return View(PaginatedList<ThreadsBrowseModel>.Create(threads, page, 10, GetGenres(this.data), genre, sort, type, liked, u));
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(string Id)
        {
            var thread = await data.Threads.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(User);

            if (thread == null) return Json(new { isValid = false, msg = Threads.Inexistent });

            if (user == null || user.Id != thread.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            return Json(new { isValid = true, html = RenderRazorViewToString(this, "DeleteEntity", new GlobalViewModel { Id = thread.Id, Name = "thread", Path = "/Threads/Delete/" }) });
        }

        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var thread = await data.Threads.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(this.User);

            if (thread == null) return Json(new { isValid = false, msg = Threads.Inexistent });

            if (user == null || user.Id != thread.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            var comments = this.data.Comments.Where(c => c.ThreadId == Id);
            data.RemoveRange(comments);
            data.Threads.Remove(thread);
            await data.SaveChangesAsync();

            return Json(new { isValid = true, html = RenderRazorViewToString(this, "DeletedSuccessfully"), entity = "thread" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(string type = "story", string d = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var model = new CreateThreadFormModel();

            var tt = type == "story" ? 1 : 2;

            var genreIds = new int[3];

            if (d != null)
            {
                var draft = this.data.Drafts.FirstOrDefault(x => x.Id == d);
                model = new CreateThreadFormModel
                {
                    Title = draft.Title,
                    Content = draft.Content,
                    TypeId = tt,
                };

                genreIds[0] = draft.FirstGenre != 0 ? draft.FirstGenre : 0;
                genreIds[1] = draft.SecondGenre != 0 ? draft.SecondGenre : 0;
                genreIds[2] = draft.ThirdGenre != 0 ? draft.ThirdGenre : 0;
            }

            switch (tt)
            {
                case 1:
                    {
                        model.Type = "Story";
                        model.TypeId = tt;
                        model.GenreIds = genreIds.ToList();
                        model.Genres = GetGenres(this.data, 1);
                        model.Drafts = this.data.Drafts
                            .Where(d => d.AuthorId == user.Id && d.ThreadTypeId == 1)
                            .Select(d => new DraftViewModel
                            {
                                Id = d.Id,
                                Title = d.Title,
                                TypeId = d.ThreadTypeId,
                                CreatedOn = d.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"),
                            })
                            .ToList();
                        break;
                    }
                case 2:
                    {
                        model.Type = "Poem";
                        model.TypeId = tt;
                        model.GenreIds = genreIds.ToList();
                        model.Genres = GetGenres(this.data, 2);
                        model.Drafts = this.data.Drafts
                            .Where(d => d.AuthorId == user.Id && d.ThreadTypeId == 2)
                            .Select(d => new DraftViewModel
                            {
                                Id = d.Id,
                                Title = d.Title,
                                TypeId = d.ThreadTypeId,
                                CreatedOn = d.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"),
                            })
                            .ToList();
                        break;
                    }
                default:
                    break;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateThreadFormModel model)
        {
            //Model validation
            switch (model.TypeId)
            {
                case 1:
                    {
                        if (model.Title == null) this.ModelState.AddModelError(nameof(model.Title), Threads.TitleRequired);
                        else
                        {
                            if(model.Title.Length < Threads.TitleMinLength || model.Title.Length > Threads.TitleMaxLength)
                                this.ModelState.AddModelError(nameof(model.Title), Threads.TitleLengthMsg);
                        }

                        if (model.GenreIds == null) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
                        else
                        {
                            if (model.GenreIds.Count > 3) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.TooMany);
                            else
                            {
                                if (InexistentGenre(model.GenreIds, this.data, model.TypeId)) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
                            }
                        }

                        if(model.Content != null)
                        {
                            if (model.Content.Length < Threads.StoryMinLength)
                                this.ModelState.AddModelError(nameof(model.Content), Threads.StoryMinLengthMsg);
                        }
                        break;
                    }
                case 2:
                    {
                        if (model.Title == null) this.ModelState.AddModelError(nameof(model.Title), Threads.TitleRequired);
                        else
                        {
                            if (model.Title.Length < Threads.TitleMinLength || model.Title.Length > Threads.TitleMaxLength)
                                this.ModelState.AddModelError(nameof(model.Title), Threads.TitleLengthMsg);
                        }

                        if (model.GenreIds == null) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
                        else
                        {
                            if (model.GenreIds.Count > 3) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.TooMany);
                            else
                            {
                                if (InexistentGenre(model.GenreIds, this.data, model.TypeId)) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
                            }
                        }

                        if (model.Content != null)
                        {
                            if (model.Content.Length < Threads.PoemMinLength)
                                this.ModelState.AddModelError(nameof(model.Content), Threads.PoemMinLengthMsg);
                        }
                        break;
                    }
                default:
                    break;
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (ModelState.IsValid)
            {
                var thread = new Thread
                {
                    ThreadTypeId = model.TypeId,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedOn = DateTime.UtcNow.ToLocalTime(),
                    AuthorId = user.Id,
                };

                var genreThreads = new List<GenreThread>();

                foreach (var genre in model.GenreIds)
                {
                    genreThreads.Add(new GenreThread
                    {
                        GenreId = genre,
                        ThreadId = thread.Id,
                    });
                }

                await this.data.GenreThreads.AddRangeAsync(genreThreads);
                await this.data.Threads.AddAsync(thread);
                await this.data.SaveChangesAsync();

                return Redirect($"/threads/read?t={thread.Id}");
            }

            switch (model.TypeId)
            {
                case 1:
                    {
                        model.Type = "Story";
                        model.TypeId = model.TypeId;
                        model.Genres = GetGenres(this.data, 1);
                        model.Drafts = this.data.Drafts
                            .Where(d => d.AuthorId == user.Id && d.ThreadTypeId == 1)
                            .Select(d => new DraftViewModel
                            {
                                Id = d.Id,
                                Title = d.Title,
                                TypeId = d.ThreadTypeId,
                                CreatedOn = d.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"),
                            })
                            .ToList();
                        break;
                    }
                case 2:
                    {
                        model.Type = "Poem";
                        model.TypeId = model.TypeId;
                        model.Genres = GetGenres(this.data, 2);
                        model.Drafts = this.data.Drafts
                            .Where(d => d.AuthorId == user.Id && d.ThreadTypeId == 2)
                            .Select(d => new DraftViewModel
                            {
                                Id = d.Id,
                                Title = d.Title,
                                TypeId = d.ThreadTypeId,
                                CreatedOn = d.CreatedOn.ToString("MM/dd/yyyy hh:mm tt"),
                            })
                            .ToList();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        [NoDirectAccess]
        public async Task<IActionResult> Edit(string Id)
        {
            var thread = await this.data.Threads.FindAsync(Id);
            var user = await this.userManager.GetUserAsync(User);

            if (thread == null) return Json(new { isValid = false, msg = Threads.Inexistent });

            if (user == null || user.Id != thread.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            var type = this.data.ThreadTypes.FirstOrDefault(tt => tt.Id == thread.ThreadTypeId).Name;

            return Json(new
            {
                isValid = true,
                html = RenderRazorViewToString(this, "Edit", new EditThreadViewModel
                {
                    Id = thread.Id,
                    Type = type,
                    TypeId = thread.ThreadTypeId,
                    Title = thread.Title,
                    Content = thread.Content
                })
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditThreadViewModel model)
        {
            //Model validation
            switch (model.TypeId)
            {
                case 1:
                    {
                        if (model.Title == null) this.ModelState.AddModelError(nameof(model.Title), Threads.TitleRequired);
                        else
                        {
                            if (model.Title.Length < Threads.TitleMinLength || model.Title.Length > Threads.TitleMaxLength)
                                this.ModelState.AddModelError(nameof(model.Title), Threads.TitleLengthMsg);
                        }

                        if (model.Content != null)
                        {
                            if (model.Content.Length < Threads.StoryMinLength)
                                this.ModelState.AddModelError(nameof(model.Content), Threads.StoryMinLengthMsg);
                        }
                        break;
                    }
                case 2:
                    {
                        if (model.Title == null) this.ModelState.AddModelError(nameof(model.Title), Threads.TitleRequired);
                        else
                        {
                            if (model.Title.Length < Threads.TitleMinLength || model.Title.Length > Threads.TitleMaxLength)
                                this.ModelState.AddModelError(nameof(model.Title), Threads.TitleLengthMsg);
                        }

                        if (model.Content != null)
                        {
                            if (model.Content.Length < Threads.PoemMinLength)
                                this.ModelState.AddModelError(nameof(model.Content), Threads.PoemMinLengthMsg);
                        }
                        break;
                    }
                default:
                    break;
            }

            if (!ModelState.IsValid) return Json(new { isValid = false, html = RenderRazorViewToString(this, "Edit", model) });

            var thread = await this.data.Threads.FindAsync(model.Id);

            if(model.TypeId != 3) thread.Title = model.Title;
            thread.Content = model.Content;

            this.data.SaveChanges();

            return Json(new
            {
                isValid = true,
                entity = "thread",
                title = model.Title,
                content = model.Content,
                msg = Threads.Updated
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Like(string threadId, bool fromList = false)
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
            var msg = "";

            if (ut != null)
            {
                this.data.UserThreads.Remove(ut);
                thread.Points--;
                liked = false;
                msg = Threads.Disliked;
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
                msg = Threads.Liked;
            }
            await this.data.SaveChangesAsync();

            var points = this.data.Threads.FirstOrDefault(t => t.Id == threadId).Points;

            if (fromList)
            {
                return Json(new { fromList = fromList, liked = liked, msg = msg, heart = $"heart{threadId}" });
            }

            var tvm = new ThreadViewModel
            {
                //comment
                Id = threadId,
                Points = points,
                Liked = liked,
            };

            return Json(new { liked = liked, msg = msg, html = RenderRazorViewToString(this, "_ThreadLikesPartial", tvm) });
        }

        //Functions
        private static bool InexistentGenre(ICollection<int> genreIds, _4draftsDbContext data, int typeId)
        {
            foreach (var genreId in genreIds)
            {
                if (data.Genres.FirstOrDefault(g => g.Id == genreId && g.GenreTypeId == typeId) == null) return true;
            }
            return false;
        }

        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,
                _4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);

        private static List<GenresBrowseModel> GetGenres(_4draftsDbContext data, int typeId = 0, string threadId = null)
        {
            var genres = new List<GenresBrowseModel>();

            if(threadId == null)
            {
                if (typeId != 0)
                {
                    genres = data
                      .Genres
                      .Where(g => g.GenreTypeId == typeId)
                      .Select(c => new GenresBrowseModel
                      {
                          Id = c.Id,
                          Name = c.Name,
                          SimplifiedName = c.SimplifiedName,
                          Description = c.Description,
                          GenreType = c.GenreTypeId,
                      })
                      .ToList();
                }
                else
                {
                    genres = data
                      .Genres
                      .Select(c => new GenresBrowseModel
                      {
                          Id = c.Id,
                          Name = c.Name,
                          SimplifiedName = c.SimplifiedName,
                          Description = c.Description,
                          GenreType = c.GenreTypeId,
                      })
                      .ToList();
                }
            }
            else
            {
                 genres = data
                      .Genres
                      .Where(g => data.GenreThreads.Any(x => x.ThreadId == threadId && x.GenreId == g.Id))
                      .Select(c => new GenresBrowseModel
                      {
                          Id = c.Id,
                          Name = c.Name,
                          SimplifiedName = c.SimplifiedName,
                          Description = c.Description,
                          GenreType = c.GenreTypeId,
                      })
                      .ToList();
            }

            return genres;
        }
        private static bool CommentIsLiked(string commentId, string userId, _4draftsDbContext data)
                => data.UserComments.Any(uc => uc.UserId == userId && uc.CommentId == commentId);

        private static bool ThreadIsLiked(string threadId, string userId, _4draftsDbContext data)
        => data.UserThreads.Any(ut => ut.UserId == userId && ut.ThreadId == threadId);
    }
}


