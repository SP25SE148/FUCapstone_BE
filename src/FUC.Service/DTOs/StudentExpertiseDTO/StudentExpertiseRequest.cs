namespace FUC.Service.DTOs.StudentExpertiseDTO;

public sealed record StudentExpertiseRequest(
    Guid? Id,
    Guid TechnicalAreaId);
