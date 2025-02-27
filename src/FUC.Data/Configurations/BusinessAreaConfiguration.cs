using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class BusinessAreaConfiguration : IEntityTypeConfiguration<BusinessArea>
{
    public void Configure(EntityTypeBuilder<BusinessArea> builder)
    {
        builder.ToTable(TableNames.BusinessArea);
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired();
        builder.Property(b => b.Id).HasDefaultValue(Guid.NewGuid());

        builder.HasMany(b => b.Students)
            .WithOne(s => s.BusinessArea)
            .HasForeignKey(s => s.BusinessAreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
