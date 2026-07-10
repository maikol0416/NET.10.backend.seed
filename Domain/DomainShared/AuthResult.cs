namespace Domain.DomainShared;

/// <summary>
/// Resultado agnóstico de una operación de autenticación.
/// El dominio no sabe nada de Identity ni JWT — solo sabe que la operación
/// fue exitosa o no, y devuelve los datos mínimos necesarios.
/// </summary>
public record AuthResult(
    bool Success,
    string? Token,
    string? Email,
    string? FullName,
    DateTime? Expiration,
    IEnumerable<string>? Errors,
    IEnumerable<string>? Roles = null,
    IEnumerable<RolePermissionsSummary>? RolePermissions = null,
    Guid? CompanyId = null
);
