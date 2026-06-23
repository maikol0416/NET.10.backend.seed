namespace Domain.DomainShared;

public abstract class Entity
{
    public Entity()
    {
        Id = Guid.NewGuid();
        Status = StatusEnum.Active.ToString();
        CreatedAt = DateTime.UtcNow;
        UpdateAt = null;
    }
    public Guid Id { get; protected set; }
    public string Status { get; protected set;}
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdateAt { get; protected set; }
}
