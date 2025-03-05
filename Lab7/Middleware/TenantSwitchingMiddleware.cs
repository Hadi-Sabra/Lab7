using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Middleware
{
    public class TenantSwitchingMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantSwitchingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Retrieve tenant identifier from the request (e.g., from a header, subdomain, or claim)
            var tenantId = context.Request.Headers["Tenant-Id"].FirstOrDefault();

            if (!string.IsNullOrEmpty(tenantId))
            {
                // Set the schema based on the tenant identifier
                SetTenantSchema(tenantId, context);
            }

            // Continue with the request pipeline
            await _next(context);
        }

        private void SetTenantSchema(string tenantId, HttpContext context)
        {
            var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();

            // Set the schema dynamically based on the tenant identifier
            dbContext.Database.ExecuteSqlRaw($"SET search_path TO {tenantId}");
        }
    }
}