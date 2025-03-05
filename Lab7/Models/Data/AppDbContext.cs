using EnrollmentService.Models;
using Lab7.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) 
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Course> Courses { get; set; } 
    public DbSet<Student> Students { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Determine the tenant schema dynamically
        var tenantSchema = GetTenantSchemaFromContext();
        modelBuilder.HasDefaultSchema(tenantSchema);  // This will set the schema dynamically
    }

    private string GetTenantSchemaFromContext()
    {
        // Logic to determine the schema based on the tenant
        var tenantId = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "BranchId")?.Value;
        if (tenantId == null) 
        {
            throw new Exception("Tenant not found");
        }

        // Assuming tenant schema is based on BranchId
        return $"branch_{tenantId}";  // Example: "branch_1", "branch_2", etc.
    }
}