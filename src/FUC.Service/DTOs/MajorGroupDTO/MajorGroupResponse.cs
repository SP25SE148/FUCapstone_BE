namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record MajorGroupResponse( Guid Id,
    string Name,
    string Code,
    string? Description,
    bool IsDeleted,
    DateTime? DeletedAt);
