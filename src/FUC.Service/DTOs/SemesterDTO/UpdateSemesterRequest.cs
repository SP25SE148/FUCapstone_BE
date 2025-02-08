using System.Runtime.InteropServices.JavaScript;

namespace FUC.Service.DTOs.SemesterDTO;

public record UpdateSemesterRequest(
    string Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate);
