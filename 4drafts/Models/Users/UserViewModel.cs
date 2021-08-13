using _4drafts.Models.Comments;
using _4drafts.Models.Threads;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; init; }

        public string Username { get; set; }

        [Display(Name = "Avatar URL")]
        [Url]
        [RegularExpression(ImageUrlRegex,
         ErrorMessage = "Invalid image URL")]
        public string AvatarUrl { get; set; }

        [Display(Name = "First name")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Range(1, 100)]
        public int? Age { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string RegisteredOn { get; set; }

        [MaxLength(20)]
        public string Occupation { get; set; }

        [MaxLength(6)]
        public string Gender { get; set; }
        [Url]
        public string Website { get; set; }
        [Url]
        public string Discord { get; set; }
        [Url]
        public string Twitter { get; set; }
        [Url]
        public string Facebook { get; set; }
        [Url]
        public string Instagram { get; set; }

        [Display(Name = "About me")]
        [MaxLength(500)]
        public string AboutMe { get; set; }
        public int ThreadCount { get; set; }
        public int CommentCount { get; set; }
        public List<ThreadsBrowseModel> Threads { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<ThreadsBrowseModel> LikedThreads { get; set; }
        public List<CommentViewModel> LikedComments { get; set; }

    }
}
