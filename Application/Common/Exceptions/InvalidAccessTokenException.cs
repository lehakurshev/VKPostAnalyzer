namespace Application.Common.Exceptions;

public class InvalidAccessTokenException : Exception
{
    public InvalidAccessTokenException() : base() { }

    public InvalidAccessTokenException(string message) : base(message) { }

    public InvalidAccessTokenException(string message, Exception inner) : base(message, inner) { }
}