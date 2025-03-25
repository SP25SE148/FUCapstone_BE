using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Processor.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(g => g.Id);

        builder.Property(x => x.CreatedDate)
            .IsRequired()
            .HasDefaultValue(DateTime.Now)
            .HasColumnType("timestamp");
    }
}
