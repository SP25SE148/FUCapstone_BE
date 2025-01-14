namespace FUC.Data.Abstractions.Entities;
public interface IDateTracking
{
    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
