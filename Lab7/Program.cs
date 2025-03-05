using Lab7;
using Lab7.Configuration;
using Lab7.Data;
using Lab7.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Azure.Storage.Blobs;
using Lab7.Middleware;
using Microsoft.Extensions.FileProviders;
using UniversityAPI.Services; // Required for Azure Blob Storage

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<RabbitMqService>();

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Keycloak settings
builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection("Keycloak"));

var keycloakSettings = builder.Configuration
    .GetSection("Keycloak")
    .Get<KeycloakSettings>();

// Configure Authentication with Keycloak
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = keycloakSettings.Authority;
    options.Audience = keycloakSettings.ClientId;
    options.RequireHttpsMetadata = keycloakSettings.RequireHttpsMetadata;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        RoleClaimType = "realm_access",  // Keycloak roles are inside `realm_access`
        NameClaimType = "preferred_username"
    };
});

// Add Authorization with role-based policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => 
                c.Type == "realm_access" && c.Value.Contains("Student"))));

    options.AddPolicy("TeacherOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => 
                c.Type == "realm_access" && c.Value.Contains("Teacher"))));
});

// Register application services
builder.Services.AddScoped<AuthService>();

// Register Azure Blob Storage if needed
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));
builder.Services.AddScoped<BlobStorageService>();

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets")),
    RequestPath = "/assets"
});

// Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<TenantSwitchingMiddleware>();  // Add the tenant middleware

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
