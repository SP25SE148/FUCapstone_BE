namespace FUC.Service.DTOs.CampusDTO;

public sealed record UpdateCampusRequest(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Email);
