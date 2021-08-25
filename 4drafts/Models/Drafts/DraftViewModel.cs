using _4drafts.Data;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Drafts
{
    public class DraftViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(DataConstants.Threads.TitleMaxLength)]
        public string Title { get; set; }

        [MaxLength(DataConstants.Threads.DescriptionMaxLength)]
        public string Description { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }

        public string AuthorId { get; set; }
    }
}
