namespace Domain.Ports.Identity;

/// <summary>
/// Puerto del dominio para generación de tokens de autenticación.
/// El dominio dicta el contrato; la infraestructura lo implementa con JWT.
/// </summary>
public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(string userId, string email, IList<string> roles, Guid? companyId = null);
}
