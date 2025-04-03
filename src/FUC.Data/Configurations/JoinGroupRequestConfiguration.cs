using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class JoinGroupRequestConfiguration : IEntityTypeConfiguration<JoinGroupRequest>
{
    public void Configure(EntityTypeBuilder<JoinGroupRequest> builder)
    {
        builder.ToTable(TableNames.JoinGroupRequest);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).HasConversion(
                v => v.ToString(),
                v => (JoinGroupRequestStatus)Enum.Parse(typeof(JoinGroupRequestStatus), v))
            .HasDefaultValue(JoinGroupRequestStatus.Pending);


        builder.HasOne(gr => gr.Group)
            .WithMany(g => g.JoinGroupRequests)
            .HasForeignKey(gr => gr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gr => gr.Student)
            .WithMany(s => s.JoinGroupRequests)
            .HasForeignKey(gr => gr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gr => gr.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(gr => gr.UpdatedDate)
            .HasColumnType("timestamp");
    }
}
