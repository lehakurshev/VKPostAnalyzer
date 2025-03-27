namespace Application.Common.Exceptions;

public class VkApiException : Exception
{
    public VkApiException() : base() { }

    public VkApiException(string message) : base(message) { }

    public VkApiException(string message, Exception inner) : base(message, inner) { }
}