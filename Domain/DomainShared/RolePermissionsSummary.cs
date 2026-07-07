namespace Domain.DomainShared;

/// <summary>
/// Desglose de los módulos permitidos para un rol puntual. Un usuario puede
/// tener varios roles, cada uno con su propia lista de módulos — el front
/// usa este desglose para dejar al usuario elegir con qué rol trabajar.
/// </summary>
public record RolePermissionsSummary(
    string RoleId,
    string RoleName,
    IEnumerable<ModuleEnum> Permissions
);
