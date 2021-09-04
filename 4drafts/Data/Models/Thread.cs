﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static _4drafts.Data.DataConstants;

namespace _4drafts.Data.Models
{
    public class Thread
    {
        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(Threads.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(Threads.DescriptionMaxLength)]
        public string Description { get; set; }

        public DateTime CreatedOn { get; init; }

        public int Points { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        [Required]
        public int GenreId { get; set; }

        public Genre Genre { get; set; }

        public ICollection<Comment> Comments { get; init; } = new List<Comment>();
        public ICollection<UserThread> UserThreads { get; set; } = new List<UserThread>();
    }
}
