using Domain.DomainShared;

namespace Domain.Ports.Identity;

/// <summary>
/// Puerto del dominio para operaciones de autenticación.
/// El dominio define QUÉ necesita (login, register) sin saber CÓMO se implementa.
/// La infraestructura lo implementa usando ASP.NET Core Identity + JWT.
/// </summary>
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string fullName, string role);
    Task<AuthResult> CreateRoleAsync(string roleName);
}
