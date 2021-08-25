using _4drafts.Data;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class EditThreadViewModel
    {
        [Display(Name = "Thread ID")]
        public string Id { get; set; }

        [Required]
        [StringLength(DataConstants.Threads.TitleMaxLength,
            MinimumLength = DataConstants.Threads.TitleMinLength,
            ErrorMessage = DataConstants.Threads.TitleMinLengthMsg)]
        public string Title { get; set; }

        [MaxLength(DataConstants.Threads.DescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [MinLength(DataConstants.Threads.ContentMinLength, ErrorMessage = DataConstants.Threads.ContentMinLengthMsg)]
        public string Content { get; set; }
    }
}
