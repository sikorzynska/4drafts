using _4drafts.Data;
using _4drafts.Models.Categories;
using _4drafts.Models.Threads;
using System.Collections.Generic;

namespace _4drafts.Services
{
    public interface IEntityGetter
    {
        public IEnumerable<CategoriesBrowseModel> GetCategories(_4draftsDbContext data);

        public int ThreadCommentCount(string threadId, _4draftsDbContext data);

        public List<ThreadsBrowseModel> CategoryThreads(int categoryId, _4draftsDbContext data);

        public ThreadViewModel GetLastThreadInCategory(int categoryId, _4draftsDbContext data);
    }
}
