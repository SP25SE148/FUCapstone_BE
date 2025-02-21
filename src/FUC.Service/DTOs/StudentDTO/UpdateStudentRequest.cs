using DocumentFormat.OpenXml.CustomProperties;
using FUC.Data.Entities;
using FUC.Service.DTOs.StudentExpertiseDTO;

namespace FUC.Service.DTOs.StudentDTO;

public sealed record UpdateStudentRequest(
    Guid BusinessAreaId,
    IReadOnlyList<StudentExpertiseRequest> StudentExpertises);
