using System.Collections.Generic;

namespace _4drafts.Models.Threads
{
    public class ThreadsBrowseModel
    {
            public string Id { get; set; }

            public string Title { get; set; }
            public string Content { get; set; }

            public string CreatedOn { get; set; }

            public int Views { get; set; }

            public int Points { get; set; }

            public string AuthorId { get; set; }

            public string AuthorName { get; set; }

            public string ThreadTypeName { get; set; }

            public string ThreadTypeSimplified { get; set; }

            public int ThreadTypeId { get; set; }

            public int Responses { get; set; }

            public string AuthorAvatarUrl { get; set; }

            public IEnumerable<int> GenreIds { get; set; }

            public IEnumerable<string> GenreNames { get; set; }
            public List<string> GenresSimplified { get; set; }
            public List<string> GenreDescriptions { get; set; }

            public int CommentCount { get; set; }
    }
}
