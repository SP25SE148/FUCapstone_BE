namespace FUC.Service.DTOs.MajorDTO;

public sealed record MajorResponse(
    string Id,
    string MajorGroupId,
    string Name,
    string? Description,
    bool IsDeleted,
    DateTime? DeletedAt);
