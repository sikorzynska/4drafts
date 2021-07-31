using _4drafts.Models.Categories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Models.Threads
{
    public class CreateThreadFormModel
    {
        [Display(Name = "Category")]
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }

        public IEnumerable<CategoriesBrowseModel> Categories { get; set; }

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
