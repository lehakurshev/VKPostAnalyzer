using Application;
using Domain;
using Microsoft.OpenApi.Models;
using Persistence;
using WebApi.Middleware;


namespace WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddApplication();
        services.AddPersistence(Configuration);
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            var apiVersion = EnvironmentVariables.ApiVersion ?? "v1";
            c.SwaggerDoc("-", new OpenApiInfo { Title = "My API", Version = apiVersion });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseMiddleware<LoggingMiddleware>();
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/-/swagger.json", "-");
            c.RoutePrefix = string.Empty; 
        });
        
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigin");
        app.UseWebSockets();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}