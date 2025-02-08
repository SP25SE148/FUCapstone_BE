using System.Runtime.InteropServices.JavaScript;

namespace FUC.Service.DTOs.SemesterDTO;

public sealed record SemesterResponse(
    string Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    bool IsDeleted,
    DateTime CreatedDate, 
    DateTime? UpdatedDate, 
    string CreatedBy, 
    string? UpdatedBy, 
    DateTime? DeletedAt);
