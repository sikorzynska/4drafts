using System.Collections.Generic;

namespace _4drafts.Models.Comments
{
    public class CommentQueryModel
    {
        public string ThreadId { get; set; }

        List<CommentViewModel> Comments { get; set; }
    }
}
