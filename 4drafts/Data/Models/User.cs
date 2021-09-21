using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisteredOn { get; init; } = DateTime.UtcNow;

        public string AvatarUrl { get; set; }

        [MaxLength(Users.UsernameMaxLength)]
        public string FirstName { get; set; }

        [MaxLength(Users.UsernameMaxLength)]
        public string LastName { get; set; }

        [Range(Users.AgeMin, Users.AgeMax)]
        public int? Age { get; set; }

        [MaxLength(Users.UsernameMaxLength)]
        public string Occupation { get; set; }

        [MaxLength(Users.GenderMaxLength)]
        public string Gender { get; set; }

        public string Website { get; set; }

        public string Youtube { get; set; }

        public string Twitter { get; set; }

        public string Instagram { get; set; }

        public string Facebook { get; set; }

        public string Patreon { get; set; }

        [MaxLength(Users.AboutMeMaxLength)]
        public string AboutMe { get; set; }

        public int Points { get; set; }

        public ICollection<Draft> Drafts { get; set; } = new List<Draft>();
        public ICollection<Thread> Threads { get; set; } = new List<Thread>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<UserThread> UserThreads { get; set; } = new List<UserThread>();
        public ICollection<UserComment> UserComments { get; set; } = new List<UserComment>();
    }
}
