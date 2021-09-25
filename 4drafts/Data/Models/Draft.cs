using System;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Data.Models
{
    public class Draft
    {
        [Key]
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ThreadType ThreadType { get; set; }

        [Required]
        public int ThreadTypeId { get; set; }

        public string PromptId { get; set; }

        public string Prompt { get; set; }

        [Required]
        [MaxLength(Threads.TitleMaxLength)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        public int FirstGenre { get; set; }
        public int SecondGenre { get; set; }
        public int ThirdGenre { get; set; }
    }
}
