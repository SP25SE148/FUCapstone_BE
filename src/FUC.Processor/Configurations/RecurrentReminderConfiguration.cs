using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FUC.Processor.Configurations;

public class RecurrentReminderConfiguration : IEntityTypeConfiguration<RecurrentReminder>
{
    public void Configure(EntityTypeBuilder<RecurrentReminder> builder)
    {
        builder.ToTable("RecurrentReminders");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Content)
            .IsRequired();

        builder.Property(r => r.RemindFor)
            .IsRequired();

        builder.Property(r => r.RecurringDay)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), v)
            )
            .HasColumnType("varchar(20)");

        builder.Property(r => r.EndDate)
            .HasColumnType("timestamp")
            .IsRequired(false);

        builder.Property(r => r.RemindTime)
            .IsRequired()
            .HasColumnType("interval");
    }
}
