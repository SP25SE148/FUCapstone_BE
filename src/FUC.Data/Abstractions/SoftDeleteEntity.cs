using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Abstractions;

public class SoftDeleteEntity : Entity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
