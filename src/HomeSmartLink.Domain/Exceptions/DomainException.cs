namespace HomeSmartLink.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class EntityNotFoundException : DomainException
{
    public string EntityType { get; }
    public string EntityId { get; }

    public EntityNotFoundException(string entityType, string entityId)
        : base($"{entityType} with ID '{entityId}' was not found")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}

public class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException(string message) : base(message) { }
}

public class UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string message = "You are not authorized to perform this operation")
        : base(message) { }
}

public class ValidationDomainException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationDomainException(string propertyName, string error)
        : base($"Validation failed for {propertyName}: {error}")
    {
        Errors = new Dictionary<string, string[]> { { propertyName, [error] } };
    }

    public ValidationDomainException(IDictionary<string, string[]> errors)
        : base("Validation failed for multiple properties")
    {
        Errors = errors.AsReadOnly();
    }
}
