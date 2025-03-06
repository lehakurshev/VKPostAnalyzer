using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<LetterCount> LetterCounts { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}