using FUC.Data.Constants;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FUC.Data.Configurations;

public sealed class TemplateDocumentConfiguration : IEntityTypeConfiguration<TemplateDocument>

{
    public void Configure(EntityTypeBuilder<TemplateDocument> builder)
    {
        builder.ToTable(TableNames.TemplateDocument);

        builder.Property(t => t.FileName).IsRequired();
        builder.Property(t => t.Id).HasDefaultValue(Guid.NewGuid());
        builder.Property(t => t.FileUrl).IsRequired();
        builder.Property(t => t.IsActive).IsRequired();
    }
}
