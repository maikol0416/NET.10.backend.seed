namespace Domain.DomainShared;

public record ValueObject
{
    public ValueObject()
    {
        Created = DateTime.UtcNow;
    }
    public DateTime Created { get; set; }
}
