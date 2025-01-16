using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class MajorGroupConfiguration : IEntityTypeConfiguration<MajorGroup>
{
    public void Configure(EntityTypeBuilder<MajorGroup> builder)
    {
        builder.ToTable(TableNames.MajorGroup);
        builder.HasKey(mg => mg.Id);
        builder.Property(mg => mg.Code).IsRequired();
        builder.Property(mg => mg.Name).IsRequired();
    }
}
