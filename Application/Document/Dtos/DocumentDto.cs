namespace Application.Dto;

public class DocumentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Path { get; set; } = string.Empty;
    public List<SignatureDto> Signatures { get; set; } = new List<SignatureDto>();
}
