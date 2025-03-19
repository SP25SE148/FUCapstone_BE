using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable(TableNames.Campus);
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired();
        builder.Property(s => s.Address).IsRequired();
        builder.Property(s => s.Phone).IsRequired();
        builder.Property(s => s.Email).IsRequired();

        builder.Property(s => s.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.DeletedAt)
            .HasColumnType("timestamp");
    }
}
