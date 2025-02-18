using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Processor.Configurations;

public class ReminderConfigurations : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");

        builder.HasKey(g => g.Id);

        builder.Property(x => x.ReminderType).IsRequired();

        builder.Property(x => x.RemindFor).IsRequired();

        builder.Property(x => x.RemindDate).IsRequired();
    }
}
