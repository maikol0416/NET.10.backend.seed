using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Domain.Ports;
using Domain.Ports.Identity;
using Infraestructure.Identity;
using Infraestructure.Repository.Properties;
using Infraestructure.Repository.DocumentManagement;
using Infraestructure.Repository.People;

namespace Infraestructure.Entity;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionInfrastructureEf(this IServiceCollection services, IConfiguration configuration)
    {
        // Contexto de escritura (Command side)
        services.AddDbContext<EntityDbContext>(options =>
            options.UseSqlServer(configuration["Sqlserver:ConnectionString"]));

        services.AddScoped<IEntityDbContext>(provider =>
        {
            var context = provider.GetService<EntityDbContext>();
            if (context == null)
                throw new InvalidOperationException("EntityDbContext not found");
            return context;
        });

        // Contexto de solo lectura (Query side — CQRS)
        services.AddDbContext<EntityReadOnlyDbContext>(options =>
            options.UseSqlServer(configuration["Sqlserver:ConnectionString"]));

        services.AddScoped<IEntityReadOnlyDbContext>(provider =>
        {
            var context = provider.GetService<EntityReadOnlyDbContext>();
            if (context == null)
                throw new InvalidOperationException("EntityReadOnlyDbContext not found");
            return context;
        });

        // ─── Identity DbContext (separado del dominio) ───
        services.AddDbContext<IdentityAppDbContext>(options =>
            options.UseSqlServer(configuration["Sqlserver:ConnectionString"]));

        // ─── ASP.NET Core Identity ───
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configuración de contraseñas
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // Configuración de usuario
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentityAppDbContext>()
        .AddDefaultTokenProviders();

        // ─── JWT Authentication ───
        var jwtSettings = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSettings);

        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        // ─── Identity Services (Ports → Adapters) ───
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        //Repository — Command side (escritura)
        services.AddScoped<IPhysicalStructureRepository, PhysicalStructureRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();

        //Repository — Query side (solo lectura)
        services.AddScoped<IPhysicalStructureReadOnlyRepository, PhysicalStructureReadOnlyRepository>();
        services.AddScoped<IDocumentReadOnlyRepository, DocumentReadOnlyRepository>();
        services.AddScoped<IOwnerReadOnlyRepository, OwnerReadOnlyRepository>();

        return services;
    }
}
