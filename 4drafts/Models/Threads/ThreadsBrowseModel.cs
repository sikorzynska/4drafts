namespace _4drafts.Models.Threads
{
    public class ThreadsBrowseModel
    {
            public string Id { get; set; }

            public string Title { get; set; }
            public string Description { get; set; }

            public string CreatedOn { get; set; }

            public int Views { get; set; }

            public int Points { get; set; }

            public string AuthorId { get; set; }

            public string AuthorName { get; set; }

            public string AuthorAvatarUrl { get; set; }

            public int CategoryId { get; set; }

            public string CategoryName { get; set; }

            public int CommentCount { get; set; }
    }
}
