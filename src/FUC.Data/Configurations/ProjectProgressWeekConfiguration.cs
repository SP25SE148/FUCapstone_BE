using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class ProjectProgressWeekConfiguration : IEntityTypeConfiguration<ProjectProgressWeek>
{
    public void Configure(EntityTypeBuilder<ProjectProgressWeek> builder)
    {
        builder.ToTable(TableNames.ProjectProgressWeek);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.WeekNumber).IsRequired();

        builder.Property(t => t.TaskDescription).IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ProjectProgressWeekStatus)Enum.Parse(typeof(ProjectProgressWeekStatus), v));

        builder.HasOne(p => p.ProjectProgress)
            .WithMany(w => w.ProjectProgressWeeks)
            .HasForeignKey(p => p.ProjectProgressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
