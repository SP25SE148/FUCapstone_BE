using FUC.Data.Abstractions.Entities;
using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TopicAppraisalConfiguration : IEntityTypeConfiguration<TopicAppraisal>
{
    public void Configure(EntityTypeBuilder<TopicAppraisal> builder)
    {
        builder.ToTable(TableNames.TopicAppraisal);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (TopicAppraisalStatus)Enum.Parse(typeof(TopicAppraisalStatus), v));

        builder.HasOne(t => t.Supervisor)
            .WithMany(s => s.TopicAppraisals)
            .HasForeignKey(t => t.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Topic)
            .WithMany(t => t.TopicAppraisals)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
