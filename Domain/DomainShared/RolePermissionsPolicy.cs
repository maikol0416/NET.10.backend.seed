namespace Domain.DomainShared;

/// <summary>
/// Regla de negocio: el rol Administrator siempre tiene acceso a todos los
/// módulos del sistema, sin importar qué permisos tenga asignados explícitamente
/// en AspNetRoleClaims. Ningún otro rol recibe este privilegio automático.
/// </summary>
public static class RolePermissionsPolicy
{
    public const string AdministratorRoleName = "Administrator";

    /// <summary>
    /// Rol de administrador de una empresa (no de plataforma). A diferencia de
    /// Administrator, sí pertenece a una empresa y sus consultas quedan siempre
    /// acotadas a ella — no recibe módulos automáticos, es solo un rol más creado
    /// vía POST /auth/create-role.
    /// </summary>
    public const string CompanyAdministratorRoleName = "Company Administrator";

    public static bool IsAdministrator(string roleName) =>
        string.Equals(roleName, AdministratorRoleName, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Resuelve los permisos efectivos de un rol: si es Administrator, todos los
    /// módulos; para cualquier otro rol, exactamente los permisos asignados.
    /// </summary>
    public static IEnumerable<ModuleEnum> Resolve(string roleName, IEnumerable<ModuleEnum> assignedPermissions) =>
        IsAdministrator(roleName) ? Enum.GetValues<ModuleEnum>() : assignedPermissions;
}
