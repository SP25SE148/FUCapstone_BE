﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class MajorConfiguration : IEntityTypeConfiguration<Major>
{
    public void Configure(EntityTypeBuilder<Major> builder)
    {
        builder.ToTable(TableNames.Major);
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(m => m.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(m => m.DeletedAt)
            .HasColumnType("timestamp");
    }
}
