using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class TemplateDocument : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
}
