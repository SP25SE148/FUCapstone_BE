using FUC.Data.Constants;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable(TableNames.GroupMember);
        builder.HasKey(gm => gm.Id);

        builder.Property(gm => gm.Id).HasDefaultValue(Guid.NewGuid());
        builder.Property(gm => gm.IsLeader).IsRequired();
        builder.Property(gm => gm.Status)
            .HasConversion(
                v => v.ToString(),
                v => (GroupMemberStatus)Enum.Parse(typeof(GroupMemberStatus), v))
            .HasDefaultValue(GroupMemberStatus.UnderReview);


        // relationship config
        builder.HasOne(gm => gm.Group)
            .WithMany(g => g.GroupMembers)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gm => gm.Student)
            .WithMany(s => s.GroupMembers)
            .HasForeignKey(gm => gm.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gm => gm.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(gm => gm.UpdatedDate)
            .HasColumnType("timestamp");
    }
}
