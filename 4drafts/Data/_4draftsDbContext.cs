using _4drafts.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Data
{
    public class _4draftsDbContext : IdentityDbContext
    {
        public _4draftsDbContext(DbContextOptions<_4draftsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Thread> Threads { get; init; }
        public DbSet<Category> Categories { get; init; }
        public DbSet<Comment> Comments { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Thread>()
                .HasOne(t => t.Author)
                .WithMany(t => t.Threads)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Thread>()
                .HasOne(t => t.Category)
                .WithMany(t => t.Threads)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Comment>()
                .HasOne(t => t.Author)
                .WithMany(t => t.Comments)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Comment>()
                .HasOne(t => t.Thread)
                .WithMany(t => t.Comments)
                .HasForeignKey(t => t.ThreadId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
