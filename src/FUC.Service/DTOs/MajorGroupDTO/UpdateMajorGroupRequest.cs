namespace FUC.Service.DTOs.MajorGroupDTO;

public sealed record UpdateMajorGroupRequest(
    string Id,
    string Name,
    string? Description
);
