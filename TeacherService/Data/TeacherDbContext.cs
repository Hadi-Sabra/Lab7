using Microsoft.EntityFrameworkCore;
using TeacherService.Models;

namespace TeacherService.Data
{
    public class TeacherDbContext : DbContext
    {
        public TeacherDbContext(DbContextOptions<TeacherDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }
    }
}