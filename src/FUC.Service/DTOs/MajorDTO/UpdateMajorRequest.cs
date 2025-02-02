namespace FUC.Service.DTOs.MajorDTO;

public record UpdateMajorRequest(
    string Id,
    string MajorGroupId,
    string Name,
    string? Description);
