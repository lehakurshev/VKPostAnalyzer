using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityTypeConfiguration;

namespace Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<LetterCountRequestData> LetterCountRequestData { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var host       = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port       = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var db         = Environment.GetEnvironmentVariable("DB_NAME") ?? "vkanalyticsdb";
        var username         = Environment.GetEnvironmentVariable("DB_USER_NAME") ?? "postgres";
        var password   = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
        var connString = $"Host={host};Port={port};Database={db};Username={username};Password={password}";

        optionsBuilder.UseNpgsql(connString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LetterCountConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}