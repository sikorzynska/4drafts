using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Drafts
{
    public class DraftViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }
    }
}
