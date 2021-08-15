using _4drafts.Data;
using _4drafts.Models.Categories;
using _4drafts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using _4drafts.Models.Threads;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        public CategoriesController(ITimeWarper timeWarper,
            _4draftsDbContext data)
        {
            this.timeWarper = timeWarper;
            this.data = data;
        }

        public IActionResult All()
        {
            var categories = this.data.Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ThreadCount = this.data.Threads.Count(t => t.CategoryId == c.Id),
                    LastEntry = GetLastThreadInCategory(c.Id, this.data, this.timeWarper)
                })
                .OrderByDescending(c => c.ThreadCount)
                .ToList();

            return View(categories);
        }

        public IActionResult Browse(int categoryId)
        {
            if (!this.data.Categories.Any(c => c.Id == categoryId)) return NotFound();

            var name = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Name;
            var desc = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Description;

            var threads = this.data.Threads
                .Include(t => t.Category)
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CategoryId = t.CategoryId,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    AuthorAvatarUrl = t.Author.AvatarUrl,
                    CommentCount = ThreadCommentCount(t.Id, this.data),
                })
                .ToList();

            return View(new CategoryBrowseModel 
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
                .Where(t => t.CategoryId == categoryId)
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
