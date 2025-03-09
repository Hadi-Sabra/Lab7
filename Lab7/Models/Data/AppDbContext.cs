using EnrollmentService.Models;
using Lab7.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly string _tenantSchema;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) 
        : base(options)
    {
        _tenantSchema = tenantProvider.GetTenantSchema();
    }

    public DbSet<Course> Courses { get; set; } 
    public DbSet<Student> Students { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_tenantSchema);  // Dynamically set schema
    }
}