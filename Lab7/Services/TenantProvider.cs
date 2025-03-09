public interface ITenantProvider
{
    string GetTenantSchema();
}

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetTenantSchema()
    {
        var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst("BranchId")?.Value;
        
        return !string.IsNullOrEmpty(tenantId) ? $"branch_{tenantId}" : "public";  
        // Defaults to "public" schema if no branch is found
    }
}