using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.ControllerExtensions;
using static _4drafts.Data.DataConstants;
using _4drafts.Models.Threads;
using _4drafts.Services;
using _4drafts.Models.Genres;
using System.Collections.Generic;
using _4drafts.Models.Shared;

namespace _4drafts.Controllers
{
    public class UsersController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly ITimeWarper timeWarper;
        public UsersController(_4draftsDbContext data,
            UserManager<User> userManager,
            ITimeWarper timeWarper)
        {
            this.data = data;
            this.userManager = userManager;
            this.timeWarper = timeWarper;
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> Peek(string u)
        {
            var user = await this.userManager.FindByNameAsync(u);

            if (user == null) return Redirect("/");

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email,
                Points = user.Points,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),

            };

            return View(res);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Redirect("/");

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Gender = user.Gender,
                Age = user.Age,
                Occupation = user.Occupation,
                Website = user.Website,
                Youtube = user.Youtube,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Patreon = user.Patreon,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),
            };

            return View(res);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Manage(UserViewModel model)
        {
            if (model.Gender != "Male" && model.Gender != "Female" && model.Gender != string.Empty && model.Gender != null)
                this.ModelState.AddModelError(nameof(model.Gender), Users.InvalidGender);

            if (!ModelState.IsValid) return View(model);

            var userId = this.userManager.GetUserId(this.User);

            var user = await this.data.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            user.AvatarUrl = model.AvatarUrl;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.Age = model.Age;
            user.Occupation = model.Occupation;
            user.AboutMe = model.AboutMe;
            user.Website = model.Website;
            user.Youtube = model.Youtube;
            user.Facebook = model.Facebook;
            user.Twitter = model.Twitter;
            user.Instagram = model.Instagram;
            user.Patreon = model.Patreon;

            await this.data.SaveChangesAsync();

            return Redirect($"/Users/Profile?u={user.UserName}");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Threads(string t = "mine", int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var threads = new List<ThreadsBrowseModel>();

            switch (t)
            {
                case "mine":
                    {
                      threads = this.data.Threads
                      .Where(t => t.AuthorId == user.Id)
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
                          Liked = ThreadIsLiked(t.Id, user.Id, this.data),
                          AuthorId = t.AuthorId,
                          AuthorName = t.Author.UserName,
                          AuthorAvatarUrl = t.Author.AvatarUrl,
                          CommentCount = ThreadCommentCount(t.Id, this.data),
                      }).ToList();
                        break;
                    }
                case "liked":
                    {
                        threads = this.data.Threads
                        .Where(t => this.data.UserThreads.Any(x => x.ThreadId == t.Id && x.UserId == user.Id))
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
                            Liked = ThreadIsLiked(t.Id, user.Id, this.data),
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
                    break;
            }

            return View(PaginatedList<ThreadsBrowseModel>.Create(threads, page, 10, GetGenres(this.data), 0, null, 0, t));
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string u)
        {
            var user = await this.data.Users
                .Include(x => x.UserThreads)
                .FirstOrDefaultAsync(x => x.UserName == u);

            if (user == null) return NotFound();

            var res = new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RegisteredOn = user.RegisteredOn.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Gender = user.Gender,
                Age = user.Age,
                Points = user.Points,
                Occupation = user.Occupation,
                Website = user.Website,
                Youtube = user.Youtube,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Patreon = user.Patreon,
                AboutMe = user.AboutMe,
                ThreadCount = UserThreadCount(user.Id, this.data),
                CommentCount = UserCommentCount(user.Id, this.data),
            };

            return View(res);
        }

        //Functions
        private static bool ThreadIsLiked(string threadId, string userId, _4draftsDbContext data)
                => data.UserThreads.Any(ut => ut.UserId == userId && ut.ThreadId == threadId);
        private static List<GenresBrowseModel> GetGenres(_4draftsDbContext data, int typeId = 0, string threadId = null)
        {
            var genres = new List<GenresBrowseModel>();

            if (threadId == null)
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
        private static int UserThreadCount(string userId, _4draftsDbContext data)
                => data.Threads.Count(t => t.AuthorId == userId);

        private static int UserCommentCount(string userId, _4draftsDbContext data)
                => data.Comments.Count(c => c.AuthorId == userId);

        private static int ThreadCommentCount(string threadId,_4draftsDbContext data)
                => data.Comments.Count(c => c.ThreadId == threadId);
    }
}
