namespace FUC.Service.DTOs.MajorDTO;

public sealed record MajorResponse(
    Guid Id,
    Guid MajorGroupId,
    string Name,
    string Code,
    string? Description,
    bool IsDeleted,
    DateTime? DeletedAt);
