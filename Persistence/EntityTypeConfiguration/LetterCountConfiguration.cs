using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeConfiguration;

public class LetterCountConfiguration : IEntityTypeConfiguration<LetterCountRequestData>
{
    public void Configure(EntityTypeBuilder<LetterCountRequestData> builder)
    {
        builder.HasKey(letterCount => letterCount.Id);
        builder.HasIndex(letterCount => letterCount.Id).IsUnique();
    }
}