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
    }
}
