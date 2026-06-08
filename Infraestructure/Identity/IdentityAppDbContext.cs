using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Identity;

/// <summary>
/// DbContext dedicado para ASP.NET Core Identity.
/// Separado de EntityDBSets para no contaminar el modelo de dominio
/// con las tablas de Identity (AspNetUsers, AspNetRoles, etc.).
/// </summary>
public class IdentityAppDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
    {
    }
}
