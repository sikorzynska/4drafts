using _4drafts.Data;
using _4drafts.Models.Genres;
using _4drafts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using _4drafts.Models.Threads;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Controllers
{
    public class GenresController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        public GenresController(ITimeWarper timeWarper,
            _4draftsDbContext data)
        {
            this.timeWarper = timeWarper;
            this.data = data;
        }

        public IActionResult All()
        {
            var categories = this.data.Genres
                .Select(c => new GenresBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ThreadCount = this.data.Threads.Count(t => t.GenreId == c.Id),
                    LastEntry = GetLastThreadInCategory(c.Id, this.data, this.timeWarper)
                })
                .OrderByDescending(c => c.ThreadCount)
                .ToList();

            return View(categories);
        }

        public IActionResult Browse(int categoryId)
        {
            if (!this.data.Genres.Any(c => c.Id == categoryId)) return NotFound();

            var name = this.data.Genres.FirstOrDefault(c => c.Id == categoryId).Name;
            var desc = this.data.Genres.FirstOrDefault(c => c.Id == categoryId).Description;

            var threads = this.data.Threads
                .Include(t => t.Genre)
                .Where(t => t.GenreId == categoryId)
                .OrderByDescending(t => t.Points)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    GenreId = t.GenreId,
                    GenreName = t.Genre.Name,
                    GenreSimplified = t.Genre.SimplifiedName,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    AuthorAvatarUrl = t.Author.AvatarUrl,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                })
                .ToList();

            return View(new GenreBrowseModel 
            { 
                Id = categoryId,
                Name = name,
                Description = desc,
                Threads = threads,
            });
        }

        //Functions
        private static ThreadViewModel GetLastThreadInCategory(int categoryId, 
            _4draftsDbContext data, 
            ITimeWarper timeWarper)
        {
            var thread = data.Threads
                .Include(t => t.Author)
                .Where(t => t.GenreId == categoryId)
                .OrderByDescending(t => t.CreatedOn)
                .FirstOrDefault();

            var result = thread == null ? null : new ThreadViewModel
            {
                Id = thread.Id,
                Title = thread.Title,
                CreatedOn = timeWarper.TimeAgo(thread.CreatedOn),
                AuthorId = thread.AuthorId,
                AuthorName = thread.Author.UserName,
                AuthorAvatarUrl = thread.Author.AvatarUrl
            };

            return result;
        }

        private static int ThreadCommentCount(string threadId, 
            _4draftsDbContext data) 
            => data.Comments.Count(c => c.ThreadId == threadId);
    }
}
