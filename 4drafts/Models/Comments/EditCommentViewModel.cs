using System.ComponentModel.DataAnnotations;

namespace _4drafts.Models.Comments
{
    public class EditCommentViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
