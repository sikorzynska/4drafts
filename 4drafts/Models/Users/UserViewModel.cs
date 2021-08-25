using _4drafts.Data;
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
        [Url]
        [RegularExpression(DataConstants.Users.ImageUrlRegex,
         ErrorMessage = DataConstants.Users.InvalidImageUrl)]
        public string AvatarUrl { get; set; }

        [Display(Name = "First name")]
        [MaxLength(DataConstants.Users.UsernameMaxLength)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(DataConstants.Users.UsernameMaxLength)]
        public string LastName { get; set; }

        [Range(DataConstants.Users.AgeMin, DataConstants.Users.AgeMax)]
        public int? Age { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string RegisteredOn { get; set; }

        [MaxLength(DataConstants.Users.UsernameMaxLength)]
        public string Occupation { get; set; }

        [MaxLength(DataConstants.Users.GenderMaxLength)]
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
        [MaxLength(DataConstants.Users.AboutMeMaxLength)]
        public string AboutMe { get; set; }
        public int ThreadCount { get; set; }
        public int CommentCount { get; set; }
        public List<ThreadsBrowseModel> Threads { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<ThreadsBrowseModel> LikedThreads { get; set; }
        public List<CommentViewModel> LikedComments { get; set; }

    }
}
