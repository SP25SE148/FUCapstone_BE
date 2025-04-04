﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable(TableNames.Semester);
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.EndDate).IsRequired();

        builder.Property(s => s.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.DeletedAt)
            .HasColumnType("timestamp");
        builder.Property(s => s.StartDate)
            .HasColumnType("timestamp");
        builder.Property(s => s.EndDate)
            .HasColumnType("timestamp");
    }
}
