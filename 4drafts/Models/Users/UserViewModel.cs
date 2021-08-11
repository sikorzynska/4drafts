using _4drafts.Models.Threads;
using System.Collections.Generic;

namespace _4drafts.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; init; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RegisteredOn { get; set; }
        public string Gender { get; set; }
        public string Website { get; set; }
        public string Github { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string AboutMe { get; set; }
        public int ThreadCount { get; set; }
        public int CommentCount { get; set; }
        public List<ThreadsBrowseModel> Threads { get; set; }

    }
}
