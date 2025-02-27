using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class SupervisorGroupAssignmentConfiguration : IEntityTypeConfiguration<SupervisorGroupAssignment>
{
    public void Configure(EntityTypeBuilder<SupervisorGroupAssignment> builder)
    {
        builder.ToTable(TableNames.SupervisorGroupAssignment);

        builder.HasKey(s => s.Id);

        builder.Property(sga => sga.Id)
            .IsRequired().HasDefaultValue(Guid.NewGuid());

        builder.Property(sga => sga.SupervisorId)
            .IsRequired();

        // Thiết lập quan hệ với bảng Group
        builder.HasOne(sga => sga.Group)
            .WithMany()
            .HasForeignKey(sga => sga.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Thiết lập quan hệ với Supervisor (SupervisorId bắt buộc)
        builder.HasOne(sga => sga.Supervisor)
            .WithMany()
            .HasForeignKey(sga => sga.SupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Thiết lập quan hệ với Supervisor2 (Supervisor2Id có thể null)
        builder.HasOne(sga => sga.Supervisor2)
            .WithMany()
            .HasForeignKey(sga => sga.Supervisor2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
