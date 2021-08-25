using _4drafts.Data;
using _4drafts.Models.Categories;
using _4drafts.Models.Drafts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Threads
{
    public class CreateThreadFormModel
    {
        [Display(Name = "Category")]
        public int CategoryId { get; init; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

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

        public IEnumerable<CategoriesBrowseModel> Categories { get; set; }
        public IEnumerable<DraftViewModel> Drafts { get; set; }
    }
}
