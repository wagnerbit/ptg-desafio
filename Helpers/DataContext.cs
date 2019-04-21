using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Helpers {
    public class DataContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DataContext (DbContextOptions<DataContext> options) : base (options) { }

        protected override void OnModelCreating (ModelBuilder mb) {
            mb.Entity<User> ()
                .ToTable ("User");

            mb.Entity<Phone> ()
                .ToTable ("Phone");

            mb.Entity<Phone> ()
                .HasOne (u => u.User)
                .WithMany (p => p.Phones)
                .HasForeignKey (p => p.UsersFK)
                .IsRequired ()
                .OnDelete (DeleteBehavior.Cascade);

        }
    }
}