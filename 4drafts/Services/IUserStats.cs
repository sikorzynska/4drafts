using _4drafts.Data;

namespace _4drafts.Services
{
    public interface IUserStats
    {
        public int userThreadCount(string userId, _4draftsDbContext data);

        public int userCommentCount(string userId, _4draftsDbContext data);
    }
}
