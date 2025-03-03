using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class TemplateDocument : AuditableEntity
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public bool IsFile { get; set; }
}
