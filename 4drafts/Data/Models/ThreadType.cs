using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Data.Models
{
    public class ThreadType
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SimplifiedName { get; set; }
        public ICollection<Thread> Threads { get; set; } = new List<Thread>();
        public ICollection<Draft> Drafts { get; set; } = new List<Draft>();
    }
}
