using _4drafts.Models.Threads;

namespace _4drafts.Models.Genres
{
    public class GenresBrowseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SimplifiedName { get; set; }

        public string Description { get; set; }

        public int ThreadCount { get; set; }

        public ThreadViewModel LastEntry { get; set; }
    }
}
