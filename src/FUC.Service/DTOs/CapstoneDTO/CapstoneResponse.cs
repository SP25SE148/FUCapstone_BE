namespace FUC.Service.DTOs.CapstoneDTO;

public sealed record CapstoneResponse(
    string Id,
    string MajorId,
    string Name,
    int MinMember,
    int MaxMember,
    int ReviewCount,
    int DurationWeeks,
    bool IsDeleted,
    DateTime? DeletedAt);
