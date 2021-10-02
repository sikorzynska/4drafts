using _4drafts.Models.Drafts;
using _4drafts.Models.Genres;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class CreateThreadFormModel
    {
        [Display(Name = "Genre")]
        public List<int> GenreIds { get; set; }
        public List<string> GenreNames { get; set; }

        [Display(Name = "Writing Prompt")]
        public string Prompt { get; set; }
        public string PromptId { get; set; }

        public string Type { get; set; }
        public int TypeId { get; set; }

        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public List<GenresBrowseModel> Genres { get; set; }
        public List<DraftViewModel> Drafts { get; set; }
    }
}
