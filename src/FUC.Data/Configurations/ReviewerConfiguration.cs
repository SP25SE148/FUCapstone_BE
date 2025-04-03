using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class ReviewerConfiguration : IEntityTypeConfiguration<Reviewer>
{
    public void Configure(EntityTypeBuilder<Reviewer> builder)
    {
        builder.ToTable(TableNames.Reviewer);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasDefaultValue(Guid.NewGuid());

        builder.Property(r => r.Suggestion)
            .HasMaxLength(500);

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.HasOne(r => r.Supervisor)
            .WithMany(s => s.Reviewers)
            .HasForeignKey(r => r.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ReviewCalender)
            .WithMany(rc => rc.Reviewers)
            .HasForeignKey(r => r.ReviewCalenderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(r => r.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(r => r.DeletedAt)
            .HasColumnType("timestamp");
    }
}
