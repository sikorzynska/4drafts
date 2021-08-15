using _4drafts.Models.Comments;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class ThreadViewModel
    {
        [Display(Name = "Thread ID")]
        public string Id { get; init; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string Content { get; set; }

        public string CreatedOn { get; init; }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorAvatarUrl { get; set; }

        public string AuthorRegisteredOn { get; set; }

        public int AuthorThreadCount { get; set; }

        public int Views { get; set; }

        public int Points { get; set; }

        public int? CategoryId { get; set; }

        public bool Liked { get; set; }

        public List<CommentViewModel> Comments { get; set; }
    }
}