using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;
public class SupervisorConfiguration : IEntityTypeConfiguration<Supervisor>
{
    public void Configure(EntityTypeBuilder<Supervisor> builder)
    {
        builder.ToTable(TableNames.Supervisor);

        // primary key
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CampusId).IsRequired();
        builder.Property(s => s.MajorId).IsRequired();

        builder.Property(s => s.Email).IsRequired();

        builder.Property(s => s.CreatedBy).IsRequired();
        builder.Property(s => s.CreatedDate).IsRequired();

        // relationship config 
        builder.HasOne(s => s.Major)
            .WithMany(m => m.Supervisors)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Campus)
            .WithMany(m => m.Supervisors)
            .HasForeignKey(s => s.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Topics)
            .WithOne(t => t.MainSupervisor)
            .HasForeignKey(s => s.MainSupervisorId)
            .OnDelete(DeleteBehavior.Restrict);

        
        builder.HasMany(s => s.CoSupervisors)
            .WithOne(c => c.Supervisor)
            .HasForeignKey(c => c.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
