using System.Text;

namespace Domain;

public static class EnvironmentVariables
{
    static EnvironmentVariables()
    {
        DotNetEnv.Env.TraversePath().Load();
    }

    public static string? AccessToken => Environment.GetEnvironmentVariable("ACCESS_TOKEN");
    public static string? DbHost => Environment.GetEnvironmentVariable("DB_HOST");
    public static string? DbPort => Environment.GetEnvironmentVariable("DB_PORT");
    public static string? DbName => Environment.GetEnvironmentVariable("DB_NAME");
    public static string? DbUserName => Environment.GetEnvironmentVariable("DB_USER_NAME");
    public static string? DbPassword => Environment.GetEnvironmentVariable("DB_PASSWORD");
    
    public static readonly string? ApiVersion = Environment.GetEnvironmentVariable("API_VERSION");
}