﻿using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class CoSupervisorConfiguration : IEntityTypeConfiguration<CoSupervisor>

{
    public void Configure(EntityTypeBuilder<CoSupervisor> builder)
    {
        builder.ToTable(TableNames.CoSupervisor);
        builder.HasKey(cs => cs.Id);

        builder.HasOne(cs => cs.Supervisor)
            .WithMany(s => s.CoSupervisors)
            .HasForeignKey(cs => cs.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Topic)
            .WithMany(t => t.CoSupervisors)
            .HasForeignKey(cs => cs.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
