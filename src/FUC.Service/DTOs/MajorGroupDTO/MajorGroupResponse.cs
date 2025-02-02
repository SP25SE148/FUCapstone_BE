namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record MajorGroupResponse( 
    string Id,
    string Name,
    string? Description,
    bool IsDeleted,
    DateTime? DeletedAt);
