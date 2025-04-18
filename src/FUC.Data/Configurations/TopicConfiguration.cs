﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(TableNames.Topic);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValue(Guid.NewGuid());

        builder.Property(t => t.FileUrl).IsRequired();
        builder.Property(t => t.FileName).IsRequired();
        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (TopicStatus)Enum.Parse(typeof(TopicStatus), v))
            .HasDefaultValue(TopicStatus.Pending);

        builder.Property(t => t.IsAssignedToGroup)
            .HasDefaultValue(false);

        builder.Property(t => t.DifficultyLevel)
            .HasConversion<int>();

        // relationship config

        builder.HasOne(t => t.MainSupervisor)
            .WithMany(s => s.Topics)
            .HasForeignKey(t => t.MainSupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Campus)
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Capstone)
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.CapstoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Semester)
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.BusinessArea)
            .WithMany(c => c.Topics)
            .HasForeignKey(t => t.BusinessAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.DeletedAt)
            .HasColumnType("timestamp");
    }
}
