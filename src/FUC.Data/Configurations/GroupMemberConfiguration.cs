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

        builder.Property(gm => gm.IsLeader).IsRequired();
        builder.Property(gm => gm.Status)
            .HasConversion(
                v => v.ToString(),
                v => (GroupMemberStatus)Enum.Parse(typeof(GroupMemberStatus), v));

        
        // relationship config
        builder.HasOne(gm => gm.Group)
            .WithMany(g => g.GroupMembers)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gm => gm.Student)
            .WithOne(s => s.GroupMember)
            .HasForeignKey<GroupMember>(gm => gm.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
