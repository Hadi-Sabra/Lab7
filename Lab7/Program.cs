using Lab7;
using Lab7.Configuration;
using Lab7.Data;
using Lab7.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext for PostgreSQL (or your preferred database)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Keycloak settings
builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection("Keycloak"));

var keycloakSettings = builder.Configuration
    .GetSection("Keycloak")
    .Get<KeycloakSettings>();

// Configure Authentication
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
            RoleClaimType = "roles", // Ensures roles are extracted properly
            NameClaimType = "preferred_username"
        };
    });

// Add Authorization with role-based policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly", policy => policy.RequireClaim("Role", "Student"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireClaim("Role", "Teacher"));
});

// Register application services
builder.Services.AddScoped<AuthService>();

// Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Ensures database is up to date
}

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
