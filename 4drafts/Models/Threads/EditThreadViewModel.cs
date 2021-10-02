using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class EditThreadViewModel
    {
        [Display(Name = "Thread ID")]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public int TypeId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
