namespace FUC.Service.DTOs.CapstoneDTO;

public sealed record CreateCapstoneRequest(
    string Id,
    string MajorId,
    string Name,
    int MinMember,
    int MaxMember,
    int ReviewCount,
    int DurationWeeks);
