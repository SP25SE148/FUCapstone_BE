﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable(TableNames.Group);
        builder.HasKey(g => g.Id);

        builder.Property(g => g.GroupCode).IsRequired();

        builder.Property(g => g.Status)
            .HasConversion(
                v => v.ToString(),
                v => (GroupStatus)Enum.Parse(typeof(GroupStatus), v));

        // relationship config

        builder.HasOne(g => g.Major)
            .WithMany(m => m.Groups)
            .HasForeignKey(g => g.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Campus)
            .WithMany(m => m.Groups)
            .HasForeignKey(g => g.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Semester)
            .WithMany(m => m.Groups)
            .HasForeignKey(g => g.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(g => g.Capstone)
            .WithMany(m => m.Groups)
            .HasForeignKey(g => g.CapstoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.GroupMembers)
            .WithOne(gm => gm.Group)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
