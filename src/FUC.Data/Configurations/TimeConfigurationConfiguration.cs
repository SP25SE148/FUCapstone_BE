using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class TimeConfigurationConfiguration : IEntityTypeConfiguration<TimeConfiguration>
{
    public void Configure(EntityTypeBuilder<TimeConfiguration> builder)
    {
        builder.ToTable(nameof(TimeConfiguration));

        builder.HasKey(x => x.Id);

        builder.Property(b => b.TeamUpDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.TeamUpExpirationDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicForSupervisorDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicForSupervisorExpiredDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicForGroupDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicForGroupExpiredDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.ReviewAttemptDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.ReviewAttemptExpiredDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.DefendCapstoneProjectDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.DefendCapstoneProjectExpiredDate)
            .HasColumnType("timestamp");

        builder.Property(b => b.CampusId)
            .IsRequired();

        builder.HasOne(b => b.Campus)
            .WithMany(s => s.TimeConfigurations)
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Semester)
            .WithMany(t => t.TimeConfigurations)
            .HasForeignKey(s => s.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
