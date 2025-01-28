namespace FUC.Service.DTOs.CampusDTO;

public sealed record CampusResponse(
    Guid Id,
    string Name,
    string Code,
    string Address,
    string Phone,
    string Email,
    bool IsDeleted,
    DateTime? DeletedAt);
