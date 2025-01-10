namespace FUC.Data.Abstractions.Entities;

public interface IAuditableEntity {
    public DateTimeOffset CreatedOnUtc { get; set; }

    public DateTimeOffset? ModifiedOnUtc { get; set; }
}
