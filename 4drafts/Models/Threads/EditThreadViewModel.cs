using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Models.Threads
{
    public class EditThreadViewModel
    {
        [Display(Name = "Thread ID")]
        public string Id { get; set; }

        [Required]
        [StringLength(ThreadTitleMaxLength,
            MinimumLength = ThreadTitleMinLength,
            ErrorMessage = "The title field must have a minimum length of {2}.")]
        public string Title { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "The content field must have a minimum length of 20.")]
        public string Content { get; set; }
    }
}
