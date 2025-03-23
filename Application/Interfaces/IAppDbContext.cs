using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext : IDisposable
{
    DbSet<LetterCountRequestData> LetterCountRequestData { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}