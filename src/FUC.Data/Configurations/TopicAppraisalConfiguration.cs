using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TopicAppraisalConfiguration : IEntityTypeConfiguration<TopicAppraisal>
{
    public void Configure(EntityTypeBuilder<TopicAppraisal> builder)
    {
        builder.ToTable(TableNames.TopicAppraisal);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValue(Guid.NewGuid());
        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (TopicAppraisalStatus)Enum.Parse(typeof(TopicAppraisalStatus), v))
            .HasDefaultValue(TopicAppraisalStatus.Pending);

        builder.HasOne(t => t.Supervisor)
            .WithMany(s => s.TopicAppraisals)
            .HasForeignKey(t => t.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Topic)
            .WithMany(t => t.TopicAppraisals)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.AppraisalDate)
            .HasColumnType("timestamp");
    }
}
