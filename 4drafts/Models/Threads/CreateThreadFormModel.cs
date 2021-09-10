using _4drafts.Data;
using _4drafts.Models.Drafts;
using _4drafts.Models.Genres;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class CreateThreadFormModel
    {

        [Display(Name = "Genre")]
        public ICollection<int> GenreIds { get; init; }
        public ICollection<string> GenreNames { get; set; }

        [Required]
        [StringLength(DataConstants.Threads.TitleMaxLength,
            MinimumLength = DataConstants.Threads.TitleMinLength,
            ErrorMessage = DataConstants.Threads.TitleMinLengthMsg)]
        public string Title { get; set; }

        [MaxLength(DataConstants.Threads.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [MinLength(DataConstants.Threads.ContentMinLength, ErrorMessage = DataConstants.Threads.ContentMinLengthMsg)]
        public string Content { get; set; }

        public IEnumerable<GenresBrowseModel> Genres { get; set; }
        public IEnumerable<DraftViewModel> Drafts { get; set; }
    }
}
