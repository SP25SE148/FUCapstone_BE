namespace FUC.Service.DTOs.StudentDTO;

public sealed record UpdateStudentRequest(
    Guid BusinessAreaId,
    float GPA);
