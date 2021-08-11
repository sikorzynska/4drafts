using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace _4drafts.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisteredOn { get; init; } = DateTime.UtcNow;

        public string AvatarUrl { get; set; } = "https://www.angrybirdsnest.com/wp-content/uploads/2013/10/Hand-Drawn-Blue-Bird-Avatar.jpg";

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Website { get; set; }

        public string Github { get; set; }

        public string Twitter { get; set; }

        public string Instagram { get; set; }

        public string Facebook { get; set; }

        public string AboutMe { get; set; }

        public int Points { get; set; }

        public ICollection<Thread> Threads { get; init; } = new List<Thread>();
        public ICollection<Comment> Comments { get; init; } = new List<Comment>();
        public ICollection<UserThread> UserThreads { get; set; } = new List<UserThread>();
        public ICollection<UserComment> UserComments { get; set; } = new List<UserComment>();
    }
}
