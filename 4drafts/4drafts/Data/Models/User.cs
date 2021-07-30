using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace _4drafts.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisteredOn { get; init; } = DateTime.UtcNow;

        public string AvatarUrl { get; set; } = "https://i.imgur.com/pvUS8zx.png";

        public int Points { get; set; }

        public IEnumerable<Thread> Threads { get; init; } = new List<Thread>();
        public IEnumerable<Comment> Comments { get; init; } = new List<Comment>();
    }
}
