using BookstoreApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Infrastructure.Persistence
{
    public class BookstoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can configure your entity properties here if needed
            modelBuilder.Entity<Book>().HasKey(b => b.Id);
        }
    }
}
