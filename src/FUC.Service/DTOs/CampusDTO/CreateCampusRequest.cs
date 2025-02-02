namespace FUC.Service.DTOs.CampusDTO;

public sealed record CreateCampusRequest(
    string Id,
    string Name,
    string Address,
    string Phone,
    string Email);
