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
    Task<AuthResult> RegisterAsync(string email, string password, string fullName, string role, Guid? companyId);
    Task<bool> AnyAdministratorExistsAsync();
    Task<AuthResult> CreateRoleAsync(string roleName, IEnumerable<ModuleEnum>? initialPermissions = null);
    Task<PaginatedList<UserSummary>> GetUsersPaginatedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<UserSummary>> GetUsersByRoleAsync(string role, Guid? companyId);
    Task<PaginatedList<RoleSummary>> GetRolesPaginatedAsync(int pageNumber, int pageSize);
    Task<AuthResult> UpdateUserAsync(string userId, string email, string fullName, IEnumerable<string> roles, Guid? companyId);
    Task<AuthResult> DeleteUserAsync(string userId, string requestingUserId);
    Task<AuthResult> UpdateRoleAsync(string roleId, string name);
    Task<AuthResult> DeleteRoleAsync(string roleId);
    Task<AuthResult> AssignPermissionsToRoleAsync(string roleId, IEnumerable<ModuleEnum> permissions);
}
