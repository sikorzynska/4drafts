using _4drafts.Data;
using System.Linq;

namespace _4drafts.Services
{
    public class Authenticator : IAuthenticator
    {
        //comment
        private readonly _4draftsDbContext data;
        public Authenticator(_4draftsDbContext data) => this.data = data;

        public bool EmailExists(string email)
            => this.data.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower()) != null;

        public bool EmailTaken(string email) 
            => this.data.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower()) != null;

        public bool UsernameTaken(string username) 
            => this.data.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower()) != null;
    }
}
