namespace FUC.Service.DTOs.CampusDTO;

 public sealed record CampusResponse(
    string Id,
    string Name,
    string Address,
    string Phone,
    string Email,
    bool IsDeleted,
    DateTime CreatedDate, 
    DateTime? UpdatedDate, 
    string CreatedBy, 
    string? UpdatedBy, 
    DateTime? DeletedAt);
