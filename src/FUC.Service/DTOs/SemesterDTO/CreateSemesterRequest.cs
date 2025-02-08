namespace FUC.Service.DTOs.SemesterDTO;

public sealed record CreateSemesterRequest(
    string Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate);
