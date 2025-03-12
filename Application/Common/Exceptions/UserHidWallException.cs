namespace Application.Common.Exceptions;

public class UserHidWallException : Exception
{
    public UserHidWallException() : base() { }

    public UserHidWallException(string message) : base(message) { }

    public UserHidWallException(string message, Exception inner) : base(message, inner) { }
}