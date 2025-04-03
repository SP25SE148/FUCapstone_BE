using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class DefendCapstoneProjectDecisionConfiguration : IEntityTypeConfiguration<DefendCapstoneProjectDecision>
{
    public void Configure(EntityTypeBuilder<DefendCapstoneProjectDecision> builder)
    {
        builder.ToTable(TableNames.DefendCapstoneProjectDecision);

        builder.HasKey(d => d.Id);

        builder.Property(g => g.Decision)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (DecisionStatus)Enum.Parse(typeof(DecisionStatus), v));

        builder.Property(d => d.CreatedDate).HasColumnType("timestamp");
        builder.Property(d => d.UpdatedDate).HasColumnType("timestamp");

        builder.HasOne(d => d.Group)
            .WithOne(g => g.DefendCapstoneProjectDecision)
            .HasForeignKey<DefendCapstoneProjectDecision>(d => d.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Supervisor)
            .WithMany(s => s.DefendCapstoneProjectDecisions)
            .HasForeignKey(d => d.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
