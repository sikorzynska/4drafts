using System;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Data.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(CommentMaxLength)]
        public string Content { get; init; }

        public DateTime CreatedOn { get; init; }

        public int Points { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        [Required]
        public string ThreadId { get; set; }

        public Thread Thread { get; set; }
    }
}
