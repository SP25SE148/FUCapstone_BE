using FUC.Data.Enums;

namespace FUC.Service.DTOs.StudentDTO;

public sealed record StudentResponseDTO(
    string Id,
    string FullName,
    string MajorId,
    string MajorName,
    string CapstoneId,
    string CapstoneName,
    string CampusId,
    string CampusName,
    string Email,
    bool IsEligible,
    string Status
);
