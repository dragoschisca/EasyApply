namespace EasyApply.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key) : base($"{entityName} with key '{key}' was not found.") { }
}

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException(IDictionary<string, string[]> errors) : base("Validation failed.")
    {
        Errors = errors;
    }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message) { }
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}