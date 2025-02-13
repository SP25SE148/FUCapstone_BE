namespace FUC.Service.DTOs.SupervisorDTO;

public record SupervisorResponseDTO(
    string Id,
    string FullName,
    string MajorId,
    string MajorName,
    string CampusId,
    string CampusName,
    string Email
);
