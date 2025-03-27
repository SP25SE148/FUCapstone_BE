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

        builder.Property(b => b.TimeUpDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.TimeUpExpirationDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicExpiredDate)
            .HasColumnType("timestamp");

        builder.Property(b => b.SemesterId)
            .IsRequired();

        builder.Property(b => b.CapstoneId)
            .IsRequired();

        builder.HasIndex(b => new { b.CapstoneId, b.SemesterId})
            .IsUnique();

        builder.HasOne(b => b.Semester)
            .WithMany(s => s.TimeConfigurations)
            .HasForeignKey(b => b.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Capstone)
            .WithMany(s => s.TimeConfigurations)
            .HasForeignKey(b => b.CapstoneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
