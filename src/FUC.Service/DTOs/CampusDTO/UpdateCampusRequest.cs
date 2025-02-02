namespace FUC.Service.DTOs.CampusDTO;

public sealed record UpdateCampusRequest(
    string Id,
    string Name,
    string Address,
    string Phone,
    string Email);
