namespace FUC.Service.DTOs.CampusDTO;

public sealed record CreateCampusRequest(
    string Name,
    string Code,
    string Address,
    string Phone,
    string Email);
