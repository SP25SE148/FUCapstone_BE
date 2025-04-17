namespace FUC.Service.DTOs.StudentDTO;

public sealed record UpdateStudentRequest(
    Guid? BusinessAreaId,
    string? Skills,
    float? GPA);
