namespace FUC.Data.Abstractions.Entities; 

public interface IUserTracking {
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
