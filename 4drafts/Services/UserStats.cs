using _4drafts.Data;
using System.Linq;

namespace _4drafts.Services
{
    public class UserStats : IUserStats
    {
        public int userThreadCount(string userId, _4draftsDbContext data)
              => data.Threads
                .Where(t => t.AuthorId == userId)
                .Count();

        public int userCommentCount(string userId, _4draftsDbContext data)
              => data.Comments
                .Where(t => t.AuthorId == userId)
                .Count();
    }
}
