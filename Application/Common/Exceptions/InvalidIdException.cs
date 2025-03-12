namespace Application.Common.Exceptions;

public class InvalidIdException : Exception
{
    public InvalidIdException() : base() { }

    public InvalidIdException(string message) : base(message) { }

    public InvalidIdException(string message, Exception inner) : base(message, inner) { }
}