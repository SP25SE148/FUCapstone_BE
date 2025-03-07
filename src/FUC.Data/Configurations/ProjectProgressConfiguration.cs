using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class ProjectProgressConfiguration : IEntityTypeConfiguration<ProjectProgress>
{
    public void Configure(EntityTypeBuilder<ProjectProgress> builder)
    {
        builder.ToTable(TableNames.ProjectProgress);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MeetingDate).IsRequired();

        builder.HasOne(p => p.Supervisor)
            .WithMany(s => s.ProjectProgresses)
            .HasForeignKey(p => p.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Group)
            .WithMany(g => g.ProjectProgresses)
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
