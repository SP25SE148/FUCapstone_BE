using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class StudentExpertiseConfiguration : IEntityTypeConfiguration<StudentExpertise>
{
    public void Configure(EntityTypeBuilder<StudentExpertise> builder)
    {
        builder.ToTable(TableNames.StudentExpertise);

        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Student)
            .WithMany(s => s.StudentExpertises)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.TechnicalArea)
            .WithMany(t => t.StudentExpertises)
            .HasForeignKey(s => s.TechnicalAreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
