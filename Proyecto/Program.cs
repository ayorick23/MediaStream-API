using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto.Application.Services;
using Proyecto.Domain.Configuration;
using Proyecto.Domain.Interfaces;
using Proyecto.Infrastructure;
using Proyecto.Infrastructure.Persistence;
using Proyecto.Infrastructure.Persistence.Repositories;
using Proyecto.Infrastructure.Security;
using Proyecto.Domain.Entities;
using Proyecto.Infrastructure.Identity;
// Importante para ver el método AddInfrastructure

var builder = WebApplication.CreateBuilder(args);

// 1. Vincular appsettings.json con la clase MediaSettings
builder.Services.Configure<MediaSettings>(
    builder.Configuration.GetSection(MediaSettings.SectionName));

// 2. Inyectar los servicios de la capa de Infraestructura
builder.Services.AddInfrastructure();

builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppIdentityUser, IdentityRole>(options =>
{
    // Configure Identity to allow user creation without email confirmation
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    // Password settings (optional - adjust as needed)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]))
        };

    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// Creando data por defecto
using (var scope = app.Services.CreateScope())
{
    var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleRepository.RoleExistsAsync(role))
        {
            await roleRepository.CreateRole(role);
        }
    }

    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    if (!await userRepository.UserExists("admin@admin.com"))
    {
        var result = userRepository.CreateUser(new Usuario()
        {
            Email = "admin@admin.com",
            Password = "Admin123!",
            FirstName = "Admin",
            LastName = "Admin"
        }).Result;

        var resultUserToRole = userRepository.AddToRoleAsync(result, "Admin").Result;
    }
}
app.Run();