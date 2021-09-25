using _4drafts.Data;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class EditThreadViewModel
    {
        [Display(Name = "Thread ID")]
        public string Id { get; set; }

        [StringLength(DataConstants.Threads.TitleMaxLength,
            MinimumLength = DataConstants.Threads.TitleMinLength,
            ErrorMessage = DataConstants.Threads.TitleLengthMsg)]
        public string Title { get; set; }

        public string Type { get; set; }

        public int TypeId { get; set; }

        [Required]
        [MinLength(DataConstants.Threads.ContentMinLength, ErrorMessage = DataConstants.Threads.ContentMinLengthMsg)]
        public string Content { get; set; }
    }
}
