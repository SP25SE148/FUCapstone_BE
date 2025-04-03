using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class
    DefendCapstoneProjectCouncilMemberConfiguration : IEntityTypeConfiguration<DefendCapstoneProjectCouncilMember>
{
    public void Configure(EntityTypeBuilder<DefendCapstoneProjectCouncilMember> builder)
    {
        builder.ToTable(TableNames.DefendCapstoneProjectCouncilMember);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DefendCapstoneProjectInformationCalendarId).IsRequired();
        builder.Property(x => x.SupervisorId).IsRequired();
        builder.Property(g => g.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(g => g.UpdatedDate)
            .HasColumnType("timestamp");
        builder.HasOne(x => x.DefendCapstoneProjectInformationCalendar)
            .WithMany(y => y.DefendCapstoneProjectMemberCouncils)
            .HasForeignKey(x => x.DefendCapstoneProjectInformationCalendarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Supervisor)
            .WithMany(y => y.DefendCapstoneProjectMemberCouncils)
            .HasForeignKey(x => x.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
