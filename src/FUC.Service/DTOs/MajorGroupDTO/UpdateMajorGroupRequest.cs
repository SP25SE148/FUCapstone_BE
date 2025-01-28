namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record UpdateMajorGroupRequest(
    Guid Id,
    string Name,
    string Code,
    string? Description
);
