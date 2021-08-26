using _4drafts.Data.Models;
using _4drafts.Models.Authentication;

namespace _4drafts.Services
{
    public interface IAuthenticator
    {
        public bool EmailTaken(string email);
        public bool UsernameTaken(string username);
        public bool EmailExists(string email);
    }
}
