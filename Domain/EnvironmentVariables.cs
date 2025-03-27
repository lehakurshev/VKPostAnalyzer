using System.Text;

namespace Domain;

public static class EnvironmentVariables
{
    static EnvironmentVariables()
    {
        DotNetEnv.Env.TraversePath().Load();
    }

    public static string? AccessToken => Environment.GetEnvironmentVariable("ACCESS_TOKEN");
}