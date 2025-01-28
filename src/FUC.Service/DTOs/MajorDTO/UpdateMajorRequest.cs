namespace FUC.Service.DTOs.MajorDTO;

public record UpdateMajorRequest(
    Guid Id,
    Guid MajorGroupId,
    string Name,
    string Code,
    string? Description);
