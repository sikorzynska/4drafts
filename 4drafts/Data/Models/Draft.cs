using System.ComponentModel.DataAnnotations;

namespace _4drafts.Data.Models
{
    public class Draft
    {
        [Key]
        [Required]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }
    }
}
