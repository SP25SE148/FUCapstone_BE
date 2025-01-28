namespace FUC.Service.DTOs.MajorDTO;

public record CreateMajorRequest(
    Guid MajorGroupId,
    string Name,
    string Code,
    string? Description);
