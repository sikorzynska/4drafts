using _4drafts.Data;
using _4drafts.Models.Genres;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Drafts
{
    public class DraftViewModel
    {
        [Display(Name = "Genre")]
        public List<int> GenreIds { get; init; }
        public List<string> GenreNames { get; set; }

        public string Id { get; set; }

        public string TypeName { get; set; }
        public int TypeId { get; set; }

        [Required]
        [MaxLength(DataConstants.Threads.TitleMaxLength)]
        public string Title { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }
        public string FullDate { get; set; }

        public string AuthorId { get; set; }

        public List<GenresBrowseModel> Genres { get; set; }
    }
}
