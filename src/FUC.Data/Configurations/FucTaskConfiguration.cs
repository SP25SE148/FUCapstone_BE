﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class FucTaskConfiguration : IEntityTypeConfiguration<FucTask>
{
    public void Configure(EntityTypeBuilder<FucTask> builder)
    {
        builder.ToTable(TableNames.FucTask);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.AssigneeId).IsRequired();

        builder.Property(t => t.ReporterId).IsRequired();

        builder.Property(t => t.KeyTask).IsRequired();

        builder.Property(t => t.Description).IsRequired();

        builder.Property(t => t.Summary).IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (FucTaskStatus)Enum.Parse(typeof(FucTaskStatus), v))
            .HasDefaultValue(FucTaskStatus.ToDo);

        builder.Property(t => t.Priority)
            .HasConversion(
                v => v.ToString(),
                v => (Priority)Enum.Parse(typeof(Priority), v));

        builder.HasOne(t => t.ProjectProgress)
            .WithMany(p => p.FucTasks)
            .HasForeignKey(p => p.ProjectProgressId)  
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Assignee)
            .WithMany(s => s.FucTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Reporter)
            .WithMany(s => s.ReportFucTasks)
            .HasForeignKey (t => t.ReporterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.DeletedAt)
            .HasColumnType("timestamp");
        builder.Property (t => t.DueDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.CompletionDate)
            .HasColumnType("timestamp");
    }
}
