using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Models.Drafts
{
    public class DraftViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(ThreadTitleMaxLength)]
        public string Title { get; set; }

        [MaxLength(ThreadDescriptionMaxLength)]
        public string Description { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }

        public string AuthorId { get; set; }
    }
}
