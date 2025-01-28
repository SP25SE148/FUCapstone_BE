namespace FUC.Service.DTOs.CapstoneDTO;

public sealed record CapstoneResponse(
    Guid Id,
    Guid MajorId,
    string Code,
    string Name,
    int MinMember,
    int MaxMember,
    int ReviewCount,
    bool IsDeleted,
    DateTime? DeletedAt);
