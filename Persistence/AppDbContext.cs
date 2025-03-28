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
        var host       = EnvironmentVariables.DbHost;
        var port       = EnvironmentVariables.DbPort;
        var db         = EnvironmentVariables.DbName;
        var username          = EnvironmentVariables.DbUserName;
        var password   = EnvironmentVariables.DbUserName;
        var connString = $"Host={host};Port={port};Database={db};Username={username};Password={password}";

        optionsBuilder.UseNpgsql(connString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LetterCountConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}