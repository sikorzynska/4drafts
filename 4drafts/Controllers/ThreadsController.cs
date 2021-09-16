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
        public async Task<IActionResult> Browse(int genre = 0, string sort = "best", bool own = false, bool liked = false, int page = 1)
        {
            var threads = new List<ThreadsBrowseModel>();

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null)
            {
                own = false;
                liked = false;
            }

            if (genre == 0)
            {
                switch (sort)
                {
                    case "controversial":
                        {
                            threads = this.data.Threads
                            .Include(t => t.Comments)
                            .Include(t => t.GenreThreads)
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.Points)
                            .ThenByDescending(t => t.Comments.Count)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .OrderByDescending(t => t.CreatedOn)
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, ":names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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
                            .Where(t => t.GenreThreads.Any(gt => gt.GenreId == genre))
                            .Select(t => new ThreadsBrowseModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                Description = t.Description,
                                GenreIds = t.GenreThreads.Select(gt => gt.GenreId),
                                GenresSimplified = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "s:names"),
                                GenreDescriptions = GetGenreNames(t.GenreThreads.Select(gt => gt.GenreId), this.data, "desc"),
                                CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
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

            if (own || liked)
            {
                var userThreads = this.data.UserThreads
                    .Where(ut => ut.UserId == user.Id)
                    .Select(ut => ut.ThreadId);

                if (own && !liked)
                {
                    threads = threads
                        .Where(t => t.AuthorId == user.Id)
                        .ToList();
                }
                else if (!own && liked)
                {
                    threads = threads
                        .Where(t => userThreads.Any(id => id == t.Id))
                        .ToList();
                }
                else
                {
                    threads = threads
                        .Where(t => t.AuthorId == user.Id && userThreads.Any(id => id == t.Id))
                        .ToList();
                }
            }

            return View(PaginatedList<ThreadsBrowseModel>.Create(threads, page, 10, GetGenres(this.data), genre, sort, own, liked));
        }

        [HttpGet]
        public IActionResult Prompts() => View();

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
        [NoDirectAccess]
        public async Task<IActionResult> Create(string title = null, string description = null, string content = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            return View(new CreateThreadFormModel
            {
                Title = title,
                Description = description,
                Content = content,
                Genres = GetGenres(this.data),
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
            if (model.GenreIds == null) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
            else
            {
                if (model.GenreIds.Count > 3) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.TooMany);
                else
                {
                    if (InexistentGenre(model.GenreIds, this.data)) this.ModelState.AddModelError(nameof(model.GenreIds), Genres.Inexistent);
                }
            }

            var user = await this.userManager.GetUserAsync(this.User);

            if (ModelState.IsValid)
            {
                var thread = new Thread
                {
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    CreatedOn = DateTime.UtcNow.ToLocalTime(),
                    AuthorId = user.Id,
                };

                var genreThreads = new List<GenreThread>();

                foreach (var genreId in model.GenreIds)
                {
                    var gt = new GenreThread
                    {
                        GenreId = genreId,
                        ThreadId = thread.Id,
                    };

                    genreThreads.Add(gt);
                }

                await this.data.GenreThreads.AddRangeAsync(genreThreads);
                await this.data.Threads.AddAsync(thread);
                await this.data.SaveChangesAsync();

                return Json(new { isValid = true, redirectUrl = Url.ActionLink("Read", "Threads", new { t = thread.Id }) });
            }

            model.Genres = GetGenres(this.data);
            model.Drafts = this.data.Drafts
            .Where(d => d.AuthorId == user.Id)
            .Select(d => new DraftViewModel
            {
                Id = d.Id,
                Title = d.Title
            })
            .ToList();

            return Json(new { isValid = false, html = RenderRazorViewToString(this, "Create", model) });
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

            return Json(new
            {
                isValid = true,
                html = RenderRazorViewToString(this, "Edit", new EditThreadViewModel
                {
                    Id = thread.Id,
                    Title = thread.Title,
                    Description = thread.Description,
                    Content = thread.Content
                })
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(EditThreadViewModel model)
        {
            var thread = await this.data.Threads.FindAsync(model.Id);

            if (!ModelState.IsValid) return Json(new { isValid = false, html = RenderRazorViewToString(this, "Edit", model) });

            thread.Title = model.Title;
            thread.Description = model.Description;
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

            var tvm = new ThreadViewModel
            {
                Id = threadId,
                Points = points,
                Liked = liked,
            };

            return Json(new { liked = liked, msg = msg, html = RenderRazorViewToString(this, "_ThreadLikesPartial", tvm) });
        }

        //Functions

        private static bool InexistentGenre(ICollection<int> genreIds, _4draftsDbContext data)
        {
            foreach (var genreId in genreIds)
            {
                if (data.Genres.FirstOrDefault(g => g.Id == genreId) == null) return true;
            }
            return false;
        }
        private static List<string> GetGenreNames(IEnumerable<int> genreIds, _4draftsDbContext data, string prop)
        {
            var genreNames = new List<string>();

            switch (prop)
            {
                case "s:names":
                    {
                        foreach (var genreId in genreIds)
                        {
                            var name = data.Genres.FirstOrDefault(g => g.Id == genreId).SimplifiedName;
                            genreNames.Add(name);
                        }
                        break;
                    }
                case "names":
                    {
                        foreach (var genreId in genreIds)
                        {
                            var name = data.Genres.FirstOrDefault(g => g.Id == genreId).Name;
                            genreNames.Add(name);
                        }
                        break;
                    }
                case "desc":
                    {
                        foreach (var genreId in genreIds)
                        {
                            var desc = data.Genres.FirstOrDefault(g => g.Id == genreId).Description;
                            genreNames.Add(desc);
                        }
                        break;
                    }
            }

            return genreNames;
        }
        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,
                _4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);

        private static IEnumerable<GenresBrowseModel> GetGenres(_4draftsDbContext data)
                => data
                  .Genres
                  .Select(c => new GenresBrowseModel
                  {
                      Id = c.Id,
                      Name = c.Name
                  })
                  .ToList();
        private static bool CommentIsLiked(string commentId, string userId, _4draftsDbContext data)
                => data.UserComments.Any(uc => uc.UserId == userId && uc.CommentId == commentId);
    }
}


