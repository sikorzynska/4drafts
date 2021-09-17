using System.Collections.Generic;

namespace _4drafts.Data.Models
{
    public class ThreadType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SimplifiedName { get; set; }
        public ICollection<Thread> Threads { get; set; } = new List<Thread>();
    }
}
