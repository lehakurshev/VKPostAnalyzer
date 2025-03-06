using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfiguration;

public class LetterCountConfiguration : IEntityTypeConfiguration<LetterCount>
{
    public void Configure(EntityTypeBuilder<LetterCount> builder)
    {
        builder.HasKey(letterCount => letterCount.Id);
        builder.HasIndex(letterCount => letterCount.Id).IsUnique();
    }
}