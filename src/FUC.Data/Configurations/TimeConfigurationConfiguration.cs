﻿using FUC.Data.Entities;
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
        builder.Property(b => b.RegistTopicDate)
            .HasColumnType("timestamp");
        builder.Property(b => b.RegistTopicExpiredDate)
            .HasColumnType("timestamp");

        builder.Property(b => b.CampusId)
            .IsRequired();

        builder.HasOne(b => b.Campus)
            .WithMany(s => s.TimeConfigurations)
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
