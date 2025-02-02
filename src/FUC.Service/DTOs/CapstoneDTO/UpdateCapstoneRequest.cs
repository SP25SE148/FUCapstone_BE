namespace FUC.Service.DTOs.CapstoneDTO;

public record UpdateCapstoneRequest(
    string Id,
    string MajorId,
    string Name,
    int MinMember,
    int MaxMember,
    int ReviewCount);
