namespace _4drafts.Models.Comments
{
    public class CommentViewModel
    {
        public string Id { get; init; }

        public string Content { get; init; }

        public string CreatedOn { get; init; }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorAvatarUrl { get; set; }

        public string AuthorRegisteredOn { get; set; }

        public int AuthorCommentCount { get; set; }

        public string ThreadId { get; set; }
    }
}
