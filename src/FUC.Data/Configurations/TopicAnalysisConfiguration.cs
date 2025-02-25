using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class TopicAnalysisConfiguration : IEntityTypeConfiguration<TopicAnalysis>
{
    public void Configure(EntityTypeBuilder<TopicAnalysis> builder)
    {
        builder.ToTable(TableNames.TopicAnalysis);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.AnalysisResult).IsRequired();

        builder.Property(t => t.CreatedDate).HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(ta => ta.Topic)
            .WithMany(t => t.TopicAnalyses)
            .HasForeignKey(ta => ta.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
