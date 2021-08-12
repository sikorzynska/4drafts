using _4drafts.Models.Comments;
using _4drafts.Models.Threads;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; init; }
        public string Username { get; set; }

        [Display(Name = "Avatar URL")]
        public string AvatarUrl { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RegisteredOn { get; set; }
        public string Gender { get; set; }
        public string Website { get; set; }
        public string Github { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }

        [Display(Name = "About me")]
        public string AboutMe { get; set; }
        public int ThreadCount { get; set; }
        public int CommentCount { get; set; }
        public List<ThreadsBrowseModel> Threads { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<ThreadsBrowseModel> LikedThreads { get; set; }
        public List<CommentViewModel> LikedComments { get; set; }

    }
}
