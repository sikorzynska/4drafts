using System.Collections.Generic;
using _4drafts.Models.Threads;

namespace _4drafts.Models.Categories
{
    public class CategoryBrowseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ThreadsBrowseModel> Threads { get; init; }
    }
}
