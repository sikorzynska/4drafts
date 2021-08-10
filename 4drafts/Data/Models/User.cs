using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace _4drafts.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisteredOn { get; init; } = DateTime.UtcNow;

        public string AvatarUrl { get; set; } = "https://i.imgur.com/Q67mO1m.png";

        public int Points { get; set; }

        public ICollection<Thread> Threads { get; init; } = new List<Thread>();
        public ICollection<Comment> Comments { get; init; } = new List<Comment>();
        public ICollection<UserThread> UserThreads { get; set; } = new List<UserThread>();
        public ICollection<UserComment> UserComments { get; set; } = new List<UserComment>();
    }
}
