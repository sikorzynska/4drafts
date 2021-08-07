using _4drafts.Data;
using _4drafts.Models.Categories;
using _4drafts.Models.Threads;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Services
{
    public class EntityGetter : IEntityGetter
    {
        private readonly ITimeWarper timeWarper;

        public EntityGetter(ITimeWarper timeWarper)
        {
            this.timeWarper = timeWarper;
        }

        public int ThreadCommentCount(string threadId, _4draftsDbContext data) => data.Comments.Count(c => c.ThreadId == threadId);


        public IEnumerable<CategoriesBrowseModel> GetCategories(_4draftsDbContext data)
              => data
                .Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

        public ThreadViewModel GetLastThreadInCategory(int categoryId, _4draftsDbContext data)
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
                CreatedOn = this.timeWarper.TimeAgo(thread.CreatedOn),
                AuthorId = thread.AuthorId,
                AuthorName = thread.Author.UserName,
                AuthorAvatarUrl = thread.Author.AvatarUrl
            };

            return result;
        }
        
        public List<ThreadsBrowseModel> CategoryThreads(int categoryId, _4draftsDbContext data) 
             => data.Threads
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.CreatedOn)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Views = t.Views,
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    AuthorAvatarUrl = t.Author.AvatarUrl,
                    CommentCount = commentCount(t.Id, data),
                })
                .ToList();

        public static int commentCount(string threadId, _4draftsDbContext data)
        {
            var result = data.Comments.Where(c => c.ThreadId == threadId).Count();

            return result;
        }
    }
}
