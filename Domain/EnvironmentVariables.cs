using System.Text;

namespace Domain;

public static class EnvironmentVariables
{
    static EnvironmentVariables()
    {
        DotNetEnv.Env.TraversePath().Load();
    }

    public static string? AccessToken => Environment.GetEnvironmentVariable("ACCESS_TOKEN");
    public static byte[] Key => Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KEY"));
    public static byte[] Iv => Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("IV"));
}