using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public class FucTaskHistoryConfiguration : IEntityTypeConfiguration<FucTaskHistory>
{
    public void Configure(EntityTypeBuilder<FucTaskHistory> builder)
    {
        builder.ToTable(TableNames.FucTaskHistory);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Content).IsRequired();

        builder.HasOne(t => t.FucTask)
            .WithMany(x => x.FucTaskHistories)
            .HasForeignKey(t => t.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.CreatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.UpdatedDate)
            .HasColumnType("timestamp");
        builder.Property(t => t.DeletedAt)
            .HasColumnType("timestamp");
    }
}
