using FUC.Data.Abstractions;

namespace FUC.Data.Entities;
public class Supervisor : AuditableSoftDeleteEntity
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; }
    public string MajorId { get; set; }
    public string CampusId { get; set; }
    public string Email { get; set; } = string.Empty;

    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
}
