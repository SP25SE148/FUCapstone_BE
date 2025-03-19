using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable(TableNames.Student);

        // primary key
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CampusId).IsRequired();
        builder.Property(s => s.CapstoneId).IsRequired();
        builder.Property(s => s.MajorId).IsRequired();

        builder.Property(s => s.Email).IsRequired();

        builder.Property(s => s.Status).HasConversion(
                v => v.ToString(),
                v => (StudentStatus)Enum.Parse(typeof(StudentStatus), v))
            .HasDefaultValue(StudentStatus.InProgress);

        builder.Property(s => s.CreatedBy).IsRequired();
        builder.Property(s => s.CreatedDate).IsRequired();

        // relationship config 
        builder.HasOne(s => s.Major)
            .WithMany(m => m.Students)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Campus)
            .WithMany(m => m.Students)
            .HasForeignKey(s => s.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Capstone)
            .WithMany(m => m.Students)
            .HasForeignKey(s => s.CapstoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.GroupMembers)
            .WithOne(gm => gm.Student)
            .HasForeignKey(gm => gm.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.BusinessArea)
            .WithMany(b => b.Students)
            .HasForeignKey(s => s.BusinessAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.DeletedAt)
            .HasColumnType("timestamp");
    }
}
