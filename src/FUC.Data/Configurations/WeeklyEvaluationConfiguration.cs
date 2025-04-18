﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class WeeklyEvaluationConfiguration : IEntityTypeConfiguration<WeeklyEvaluation>
{
    public void Configure(EntityTypeBuilder<WeeklyEvaluation> builder)
    {
        builder.ToTable(TableNames.WeeklyEvaluation);

        builder.HasKey(t => t.Id);  

        builder.Property(t => t.ContributionPercentage).IsRequired();   

        builder.Property(t => t.Comments).IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(
                v => v.ToString(),
                v => (EvaluationStatus)Enum.Parse(typeof(EvaluationStatus), v));

        builder.HasOne(w => w.Student)
            .WithMany(s => s.WeeklyEvaluations)
            .HasForeignKey(w => w.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.ProjectProgressWeek)
            .WithMany(s => s.WeeklyEvaluations)
            .HasForeignKey(w => w.ProjectProgressWeekId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.DeletedAt)
            .HasColumnType("timestamp");
    }
}
