namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record CreateMajorGroupRequest(
    string Id,
    string Name,
    string? Description
);
