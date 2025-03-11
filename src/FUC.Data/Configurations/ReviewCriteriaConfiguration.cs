using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class ReviewCriteriaConfiguration : IEntityTypeConfiguration<ReviewCriteria>
{
    public void Configure(EntityTypeBuilder<ReviewCriteria> builder)
    {
        builder.ToTable(TableNames.ReviewCriteria);

        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id).HasDefaultValue(Guid.NewGuid());

        builder.Property(rc => rc.CapstoneId)
            .IsRequired();

        builder.Property(rc => rc.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(rc => rc.Description)
            .HasMaxLength(1000);

        builder.Property(rc => rc.Requirement)
            .HasMaxLength(1000);

        builder.Property(rc => rc.IsActive)
            .IsRequired().HasDefaultValue(true);

        builder.HasOne(rc => rc.Capstone)
            .WithMany(c => c.reviewCriterias)
            .HasForeignKey(rc => rc.CapstoneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
