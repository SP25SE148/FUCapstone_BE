namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record CreateMajorGroupRequest(
    string Name,
    string Code,
    string? Description
);
