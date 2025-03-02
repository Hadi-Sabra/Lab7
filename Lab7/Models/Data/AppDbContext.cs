using Lab7.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab7.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Student" },
                new Role { Id = 2, Name = "Teacher" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}