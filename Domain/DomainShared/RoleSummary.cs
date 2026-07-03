namespace Domain.DomainShared;

/// <summary>
/// Proyección agnóstica de un rol para listados. El dominio no expone
/// IdentityRole (detalle de Identity) — solo los datos mínimos de lectura.
/// </summary>
public record RoleSummary(
    string Id,
    string Name
);
