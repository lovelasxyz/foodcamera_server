namespace Cases.Domain.Exceptions;

public sealed class DuplicateException : DomainException
{
    public DuplicateException(string message) : base(message) { }
}
