namespace Domain.DomainShared;

/// <summary>
/// Proyección agnóstica de un usuario para listados. El dominio no expone
/// ApplicationUser (detalle de Identity) — solo los datos mínimos de lectura.
/// </summary>
public record UserSummary(
    string Id,
    string Email,
    string FullName,
    IEnumerable<string> Roles,
    IEnumerable<RolePermissionsSummary> RolePermissions,
    Guid? CompanyId
);
