using System.Security.Claims;
using Domain.DomainShared;
using Domain.Ports.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Identity;

/// <summary>
/// Implementación del puerto IAuthService usando ASP.NET Core Identity.
/// Encapsula toda la lógica técnica de Identity (UserManager, SignInManager, RoleManager)
/// detrás de la interfaz agnóstica del dominio.
/// </summary>
public class AuthService : IAuthService
{
    private const string PermissionClaimType = "Permission";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Credenciales inválidas."]
            );
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Credenciales inválidas."]
            );
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.Email!, roles, user.CompanyId);
        var rolePermissions = await BuildRolePermissionsSummaryAsync(user);

        return new AuthResult(
            Success: true,
            Token: token,
            Email: user.Email,
            FullName: user.FullName,
            Expiration: DateTime.UtcNow.AddMinutes(60),
            Errors: null,
            Roles: roles,
            RolePermissions: rolePermissions,
            CompanyId: user.CompanyId
        );
    }

    public async Task<bool> AnyAdministratorExistsAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync(RolePermissionsPolicy.AdministratorRoleName);
        return admins.Count > 0;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string role, Guid? companyId)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: ["Ya existe un usuario registrado con este email."]
            );
        }

        // Validar que el rol exista antes de crear el usuario
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    Email: null,
                    FullName: null,
                    Expiration: null,
                    Errors: [$"El rol '{role}' no existe. Debe crearlo primero."]
                );
            }
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            // Un Administrator es un usuario de plataforma: nunca queda atado a una empresa,
            // sin importar qué CompanyId le hayan pasado.
            CompanyId = RolePermissionsPolicy.IsAdministrator(role) ? null : companyId
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return new AuthResult(
                Success: false,
                Token: null,
                Email: null,
                FullName: null,
                Expiration: null,
                Errors: result.Errors.Select(e => e.Description)
            );
        }

        // Asignar rol al usuario recién creado
        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                return new AuthResult(
                    Success: false,
                    Token: null,
                    Email: null,
                    FullName: null,
                    Expiration: null,
                    Errors: roleResult.Errors.Select(e => e.Description)
                );
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtTokenService.GenerateTokenAsync(user.Id, user.Email!, roles, user.CompanyId);
        var rolePermissions = await BuildRolePermissionsSummaryAsync(user);

        return new AuthResult(
            Success: true,
            Token: token,
            Email: user.Email,
            FullName: user.FullName,
            Expiration: DateTime.UtcNow.AddMinutes(60),
            Errors: null,
            Roles: roles,
            RolePermissions: rolePermissions,
            CompanyId: user.CompanyId
        );
    }

    public async Task<AuthResult> CreateRoleAsync(string roleName, IEnumerable<ModuleEnum>? initialPermissions = null)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return Failure($"El rol '{roleName}' ya existe.");
        }

        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        // Administrator ya tiene acceso a todos los módulos por política de negocio
        // (ver RolePermissionsPolicy) — no tiene sentido guardarle claims explícitos.
        if (!RolePermissionsPolicy.IsAdministrator(roleName))
        {
            foreach (var permission in (initialPermissions ?? []).Distinct())
            {
                var claimResult = await _roleManager.AddClaimAsync(role, new Claim(PermissionClaimType, permission.ToString()));
                if (!claimResult.Succeeded)
                {
                    return Failure(claimResult.Errors.Select(e => e.Description));
                }
            }
        }

        return Success();
    }

    public async Task<PaginatedList<UserSummary>> GetUsersPaginatedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _userManager.Users.CountAsync();
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var summaries = new List<UserSummary>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var rolePermissions = await BuildRolePermissionsSummaryAsync(user);
            summaries.Add(new UserSummary(user.Id, user.Email!, user.FullName, roles, rolePermissions, user.CompanyId));
        }

        return new PaginatedList<UserSummary>(summaries, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<RoleSummary>> GetRolesPaginatedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _roleManager.Roles.CountAsync();
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var summaries = new List<RoleSummary>();
        foreach (var role in roles)
        {
            var permissions = await GetRolePermissionsAsync(role);
            summaries.Add(new RoleSummary(role.Id, role.Name!, permissions));
        }

        return new PaginatedList<RoleSummary>(summaries, totalCount, pageNumber, pageSize);
    }

    public async Task<AuthResult> UpdateUserAsync(string userId, string email, string fullName, IEnumerable<string> roles, Guid? companyId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Failure("Usuario no encontrado.");
        }

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var ownerOfEmail = await _userManager.FindByEmailAsync(email);
            if (ownerOfEmail != null && ownerOfEmail.Id != userId)
            {
                return Failure($"Ya existe otro usuario registrado con el email '{email}'.");
            }
        }

        var incomingRoles = roles?.ToList() ?? [];
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(incomingRoles).ToList();
        var rolesToAdd = incomingRoles.Except(currentRoles).ToList();

        if (rolesToRemove.Contains(RolePermissionsPolicy.AdministratorRoleName))
        {
            var adminGuardError = await ValidateNotLastAdminAsync(user);
            if (adminGuardError != null)
            {
                return Failure(adminGuardError);
            }
        }

        user.Email = email;
        user.UserName = email;
        user.FullName = fullName;
        // Un Administrator es un usuario de plataforma: nunca queda atado a una empresa,
        // sin importar qué CompanyId le hayan pasado (mismo criterio que RegisterAsync).
        user.CompanyId = incomingRoles.Any(RolePermissionsPolicy.IsAdministrator) ? null : companyId;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return Failure(updateResult.Errors.Select(e => e.Description));
        }

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return Failure(removeResult.Errors.Select(e => e.Description));
            }
        }

        if (rolesToAdd.Count > 0)
        {
            foreach (var role in rolesToAdd)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return Failure($"El rol '{role}' no existe. Debe crearlo primero.");
                }
            }

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return Failure(addResult.Errors.Select(e => e.Description));
            }
        }

        return Success();
    }

    public async Task<AuthResult> DeleteUserAsync(string userId, string requestingUserId)
    {
        if (string.Equals(userId, requestingUserId, StringComparison.OrdinalIgnoreCase))
        {
            return Failure("No puedes eliminar tu propio usuario.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Failure("Usuario no encontrado.");
        }

        var adminGuardError = await ValidateNotLastAdminAsync(user);
        if (adminGuardError != null)
        {
            return Failure(adminGuardError);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    public async Task<AuthResult> UpdateRoleAsync(string roleId, string name)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Failure("Rol no encontrado.");
        }

        if (!string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase) &&
            await _roleManager.RoleExistsAsync(name))
        {
            return Failure($"Ya existe otro rol con el nombre '{name}'.");
        }

        role.Name = name;
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    public async Task<AuthResult> DeleteRoleAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Failure("Rol no encontrado.");
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
        {
            return Failure($"No se puede eliminar el rol '{role.Name}' porque tiene {usersInRole.Count} usuario(s) asignado(s). Reasígnalos antes de eliminarlo.");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return Failure(result.Errors.Select(e => e.Description));
        }

        return Success();
    }

    public async Task<AuthResult> AssignPermissionsToRoleAsync(string roleId, IEnumerable<ModuleEnum> permissions)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Failure("Rol no encontrado.");
        }

        if (RolePermissionsPolicy.IsAdministrator(role.Name!))
        {
            return Failure("El rol Administrator siempre tiene acceso a todos los módulos; no es necesario ni posible asignarle permisos.");
        }

        var currentClaims = await _roleManager.GetClaimsAsync(role);
        var currentPermissionClaims = currentClaims.Where(c => c.Type == PermissionClaimType).ToList();

        foreach (var claim in currentPermissionClaims)
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeResult.Succeeded)
            {
                return Failure(removeResult.Errors.Select(e => e.Description));
            }
        }

        foreach (var permission in permissions.Distinct())
        {
            var addResult = await _roleManager.AddClaimAsync(role, new Claim(PermissionClaimType, permission.ToString()));
            if (!addResult.Succeeded)
            {
                return Failure(addResult.Errors.Select(e => e.Description));
            }
        }

        return Success();
    }

    /// <summary>
    /// Lee y parsea los claims de permisos (módulos) de un rol, y aplica
    /// RolePermissionsPolicy — Administrator siempre resuelve a todos los módulos,
    /// sin importar qué claims tenga guardados.
    /// Ignora silenciosamente valores que ya no existan en ModuleEnum (ej. tras un rename del enum).
    /// </summary>
    private async Task<IEnumerable<ModuleEnum>> GetRolePermissionsAsync(IdentityRole role)
    {
        var claims = await _roleManager.GetClaimsAsync(role);
        var assignedPermissions = claims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => Enum.TryParse<ModuleEnum>(c.Value, out var module) ? (ModuleEnum?)module : null)
            .Where(m => m.HasValue)
            .Select(m => m!.Value)
            .ToList();

        return RolePermissionsPolicy.Resolve(role.Name!, assignedPermissions);
    }

    /// <summary>
    /// Arma el desglose de permisos por cada rol del usuario — un usuario puede
    /// tener varios roles y el front deja elegir con cuál trabajar.
    /// </summary>
    private async Task<IEnumerable<RolePermissionsSummary>> BuildRolePermissionsSummaryAsync(ApplicationUser user)
    {
        var roleNames = await _userManager.GetRolesAsync(user);
        var summaries = new List<RolePermissionsSummary>();

        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                continue;
            }

            var permissions = await GetRolePermissionsAsync(role);
            summaries.Add(new RolePermissionsSummary(role.Id, role.Name!, permissions));
        }

        return summaries;
    }

    /// <summary>
    /// Bloquea la operación si el usuario es Administrator y es el último con ese rol —
    /// evita que el sistema se quede sin administradores.
    /// </summary>
    private async Task<string?> ValidateNotLastAdminAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(RolePermissionsPolicy.AdministratorRoleName))
        {
            return null;
        }

        var admins = await _userManager.GetUsersInRoleAsync(RolePermissionsPolicy.AdministratorRoleName);
        if (admins.Count <= 1)
        {
            return "No puedes eliminar ni quitar el rol Administrator al último administrador del sistema.";
        }

        return null;
    }

    private static AuthResult Success() => new(
        Success: true,
        Token: null,
        Email: null,
        FullName: null,
        Expiration: null,
        Errors: null
    );

    private static AuthResult Failure(string error) => Failure([error]);

    private static AuthResult Failure(IEnumerable<string> errors) => new(
        Success: false,
        Token: null,
        Email: null,
        FullName: null,
        Expiration: null,
        Errors: errors
    );
}
