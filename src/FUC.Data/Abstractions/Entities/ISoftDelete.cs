namespace FUC.Data.Abstractions.Entities; 

public interface ISoftDelete {
    public bool IsDeleted { get; set; } 

    public DateTimeOffset DeletedAt { get; set; }   
}
