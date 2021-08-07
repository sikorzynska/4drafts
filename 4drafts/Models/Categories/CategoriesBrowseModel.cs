using _4drafts.Models.Threads;

namespace _4drafts.Models.Categories
{
    public class CategoriesBrowseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ThreadCount { get; set; }

        public ThreadViewModel LastEntry { get; set; }
    }
}
