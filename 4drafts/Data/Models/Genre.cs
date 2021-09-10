using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Data.Models
{
    public class Genre
    {
        [Key]
        [Required]
        public int Id { get; init; }

        [Required]
        public string Name { get; init; }

        [Required]
        public string SimplifiedName { get; init; }

        [Required]
        public string Description { get; init; }

        public string ImageUrl { get; set; }

        public ICollection<GenreThread> GenreThreads { get; init; } = new List<GenreThread>();
    }
}
