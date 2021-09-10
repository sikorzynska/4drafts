using _4drafts.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Data
{
    public class _4draftsDbContext : IdentityDbContext<User>
    {
        public _4draftsDbContext(DbContextOptions<_4draftsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Thread> Threads { get; init; }
        public DbSet<Genre> Genres { get; init; }
        public DbSet<Comment> Comments { get; init; }
        public DbSet<Draft> Drafts { get; init; }
        public DbSet<UserThread> UserThreads { get; init; }
        public DbSet<UserComment> UserComments { get; init; }
        public DbSet<GenreThread> GenreThreads { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Thread>()
                .HasOne(t => t.Author)
                .WithMany(t => t.Threads)
                .HasForeignKey(t => t.AuthorId)
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

            builder
                .Entity<Draft>()
                .HasOne(d => d.Author)
                .WithMany(a => a.Drafts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserThread>()
                .HasKey(ut => new { ut.UserId, ut.ThreadId });

            builder.Entity<UserThread>()
                .HasOne(ut => ut.User)
                .WithMany(t => t.UserThreads)
                .HasForeignKey(ut => ut.UserId);

            builder.Entity<UserThread>()
                .HasOne(ut => ut.Thread)
                .WithMany(t => t.UserThreads)
                .HasForeignKey(ut => ut.ThreadId);

            builder.Entity<UserComment>()
                .HasKey(uc => new { uc.UserId, uc.CommentId });

            builder.Entity<UserComment>()
                .HasOne(uc => uc.User)
                .WithMany(c => c.UserComments)
                .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserComment>()
                .HasOne(uc => uc.Comment)
                .WithMany(c => c.UserComments)
                .HasForeignKey(uc => uc.CommentId);

            builder.Entity<GenreThread>()
                .HasKey(gt => new { gt.GenreId, gt.ThreadId });

            builder.Entity<GenreThread>()
                .HasOne(gt => gt.Genre)
                .WithMany(g => g.GenreThreads)
                .HasForeignKey(gt => gt.GenreId);

            builder.Entity<GenreThread>()
                .HasOne(gt => gt.Thread)
                .WithMany(t => t.GenreThreads)
                .HasForeignKey(gt => gt.ThreadId);

            base.OnModelCreating(builder);
        }
    }
}
