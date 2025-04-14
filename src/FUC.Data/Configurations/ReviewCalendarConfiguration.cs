using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class ReviewCalendarConfiguration : IEntityTypeConfiguration<ReviewCalendar>
{
    public void Configure(EntityTypeBuilder<ReviewCalendar> builder)
    {
        builder.ToTable(TableNames.ReviewCalendar);

        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id).HasDefaultValue(Guid.NewGuid());

        builder.Property(rc => rc.MajorId)
            .IsRequired();

        builder.Property(rc => rc.CampusId)
            .IsRequired();

        builder.Property(rc => rc.SemesterId)
            .IsRequired();

        builder.Property(rc => rc.Time)
            .IsRequired();

        builder.Property(rc => rc.Room)
            .IsRequired();

        builder.Property(rc => rc.Status)
            .IsRequired()
            .HasDefaultValue(ReviewCalendarStatus.Pending)
            .HasConversion(
                v => v.ToString(),
                v => (ReviewCalendarStatus)Enum.Parse(typeof(ReviewCalendarStatus), v));

        builder.HasOne(rc => rc.Topic)
            .WithMany(t => t.ReviewCalendars)
            .HasForeignKey(rc => rc.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Group)
            .WithMany(g => g.ReviewCalendars)
            .HasForeignKey(rc => rc.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Major)
            .WithMany(m => m.ReviewCalendars)
            .HasForeignKey(rc => rc.MajorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Campus)
            .WithMany(c => c.ReviewCalendars)
            .HasForeignKey(rc => rc.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Semester)
            .WithMany(s => s.ReviewCalendars)
            .HasForeignKey(rc => rc.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rc => rc.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(rc => rc.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(rc => rc.DeletedAt)
            .HasColumnType("timestamp");
        builder.Property(rc => rc.Date)
            .HasColumnType("timestamp");
    }
}
