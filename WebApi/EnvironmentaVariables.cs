namespace WebApi;

public class EnvironmentaVariables
{
    public static readonly string? FRONTEND_HOST = Environment.GetEnvironmentVariable("FRONTEND_HOST");
    public static readonly string? API_VERSION = Environment.GetEnvironmentVariable("API_VERSION");
}