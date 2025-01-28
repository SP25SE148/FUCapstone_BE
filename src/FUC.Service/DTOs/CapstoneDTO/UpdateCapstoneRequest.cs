namespace FUC.Service.DTOs.CapstoneDTO;

public record UpdateCapstoneRequest(
    Guid Id,
    Guid MajorId,
    string Code,
    string Name,
    int MinMember,
    int MaxMember,
    int ReviewCount);
