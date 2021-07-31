﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _4drafts.Data.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; init; }

        [Required]
        public string Name { get; init; }

        [Required]
        public string Description { get; init; }

        public string ImageUrl { get; set; }

        public IEnumerable<Thread> Threads { get; init; } = new List<Thread>();
    }
}
