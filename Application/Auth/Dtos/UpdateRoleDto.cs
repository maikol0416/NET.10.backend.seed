namespace Application.Auth.Dtos;

/// <summary>
/// DTO de entrada para renombrar un rol existente.
/// </summary>
public class UpdateRoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
