namespace FUC.Service.DTOs.MajorDTO;

public record CreateMajorRequest(
    string Id,
    string MajorGroupId,
    string Name,
    string? Description);
