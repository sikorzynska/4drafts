using _4drafts.Data;
using _4drafts.Models.Categories;
using _4drafts.Models.Threads;
using System.Collections.Generic;
using System.Linq;

namespace _4drafts.Services
{
    public class EntityGetter : IEntityGetter
    {
        private readonly ITimeWarper timeWarper;

        public EntityGetter(ITimeWarper timeWarper)
        {
            this.timeWarper = timeWarper;
        }

        public int ThreadCommentCount(string threadId, _4draftsDbContext data)
        {
            var result = data.Comments.Where(c => c.ThreadId == threadId).Count();

            return result;
        }

        public IEnumerable<CategoriesBrowseModel> GetCategories(_4draftsDbContext data)
              => data
                .Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

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
