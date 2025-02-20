using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TechnicalAreaConfiguration : IEntityTypeConfiguration<TechnicalArea>
{
    public void Configure(EntityTypeBuilder<TechnicalArea> builder)
    {
        builder.ToTable(TableNames.TechnicalArea);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();

        builder.HasMany(t => t.StudentExpertises)
            .WithOne(s => s.TechnicalArea)
            .HasForeignKey(s => s.TechnicalAreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
