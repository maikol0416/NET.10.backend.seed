using System.Runtime.Serialization;

namespace Domain.DomainShared;

[Serializable]
public class DomainException : Exception
{
    public DomainException()
        : base()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected DomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
