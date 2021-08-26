using _4drafts.Data;
using System.Linq;

namespace _4drafts.Services
{
    public class Authenticator : IAuthenticator
    {
        private readonly _4draftsDbContext data;
        public Authenticator(_4draftsDbContext data) => this.data = data;

        public bool EmailExists(string email)
            => this.data.Users.FirstOrDefault(u => u.Email == email) != null;

        public bool EmailTaken(string email) 
            => this.data.Users.FirstOrDefault(u => u.Email == email) != null;

        public bool UsernameTaken(string username) 
            => this.data.Users.FirstOrDefault(u => u.UserName == username) != null;
    }
}
