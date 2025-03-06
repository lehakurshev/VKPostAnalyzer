using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection
        services, IConfiguration configuration)
    {
        var connectionString = configuration["DbConnection"];
        services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); });

        
        services.AddScoped<IAppDbContext>(provider =>
            provider.GetService<AppDbContext>());

        return services;
    }
}