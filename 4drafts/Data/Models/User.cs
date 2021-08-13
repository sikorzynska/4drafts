using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisteredOn { get; init; } = DateTime.UtcNow;

        public string AvatarUrl { get; set; }

        [MaxLength(20)]
        public string FirstName { get; set; }

        [MaxLength(20)]
        public string LastName { get; set; }

        [Range(1, 100)]
        public int? Age { get; set; }

        [MaxLength(20)]
        public string Occupation { get; set; }

        [MaxLength(6)]
        public string Gender { get; set; }

        public string Website { get; set; }

        public string Discord { get; set; }

        public string Twitter { get; set; }

        public string Instagram { get; set; }

        public string Facebook { get; set; }

        [MaxLength(500)]
        public string AboutMe { get; set; }

        public int Points { get; set; }

        public ICollection<Thread> Threads { get; init; } = new List<Thread>();
        public ICollection<Comment> Comments { get; init; } = new List<Comment>();
        public ICollection<UserThread> UserThreads { get; set; } = new List<UserThread>();
        public ICollection<UserComment> UserComments { get; set; } = new List<UserComment>();
    }
}
