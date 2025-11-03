namespace Cases.Domain.Exceptions;

public sealed class InvalidInputException : DomainException
{
    public InvalidInputException(string message) : base(message) { }
}
