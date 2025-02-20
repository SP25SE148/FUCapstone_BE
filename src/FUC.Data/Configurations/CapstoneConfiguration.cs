using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class CapstoneConfiguration : IEntityTypeConfiguration<Capstone>
{
    public void Configure(EntityTypeBuilder<Capstone> builder)
    {
        builder.ToTable(TableNames.Capstone);
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.MaxMember).IsRequired();
        builder.Property(c => c.MinMember).IsRequired();
        builder.Property(c => c.ReviewCount).IsRequired();

        // relationship config
        builder.HasOne(c => c.Major)
            .WithMany(m => m.Capstones)
            .HasForeignKey(c => c.MajorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
    }
}
