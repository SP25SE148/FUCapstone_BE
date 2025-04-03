using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class
    DefendCapstoneProjectInformationCalendarConfiguration : IEntityTypeConfiguration<
    DefendCapstoneProjectInformationCalendar>
{
    public void Configure(EntityTypeBuilder<DefendCapstoneProjectInformationCalendar> builder)
    {
        builder.ToTable(TableNames.DefendCapstoneProjectInformationCalendar);

        builder.Property(x => x.TopicId).IsRequired();
        builder.Property(x => x.CampusId).IsRequired();
        builder.Property(x => x.SemesterId).IsRequired();
        builder.Property(x => x.DefendAttempt).IsRequired();
        builder.Property(x => x.Location).IsRequired();
        builder.Property(x => x.IsUploadedThesisMinute).HasDefaultValue(false);
        builder.Property(x => x.DefenseDate).HasColumnType("timestamp");
        builder.Property(g => g.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(g => g.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(gm => gm.Status)
            .HasConversion(
                v => v.ToString(),
                v => (DefendCapstoneProjectCalendarStatus)Enum.Parse(typeof(DefendCapstoneProjectCalendarStatus), v))
            .HasDefaultValue(DefendCapstoneProjectCalendarStatus.NotStarted);

        builder.HasOne(x => x.Campus)
            .WithMany(y => y.DefendCapstoneProjectInformationCalendars)
            .HasForeignKey(x => x.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Semester)
            .WithMany(y => y.DefendCapstoneProjectInformationCalendars)
            .HasForeignKey(x => x.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Topic)
            .WithMany(y => y.DefendCapstoneProjectInformationCalendars)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
