using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TopicRequestConfiguration : IEntityTypeConfiguration<TopicRequest>
{
    public void Configure(EntityTypeBuilder<TopicRequest> builder)
    {
        builder.ToTable(TableNames.TopicRequest);

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Id).HasDefaultValue(Guid.NewGuid()).IsRequired();
        builder.Property(tr => tr.SupervisorId).IsRequired();
        builder.Property(tr => tr.GroupId).IsRequired();
        builder.Property(tr => tr.TopicId).IsRequired();

        builder.Property(tr => tr.Status)
            .HasConversion(
                v => v.ToString(),
                v => (TopicRequestStatus)Enum.Parse(typeof(TopicRequestStatus), v))
            .HasDefaultValue(TopicRequestStatus.UnderReview);

        builder.HasOne(tr => tr.Supervisor)
            .WithMany(s => s.TopicRequests)
            .HasForeignKey(tr => tr.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Group)
            .WithMany(g => g.TopicRequests)
            .HasForeignKey(tr => tr.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.Topic)
            .WithMany(t => t.TopicRequests)
            .HasForeignKey(tr => tr.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
