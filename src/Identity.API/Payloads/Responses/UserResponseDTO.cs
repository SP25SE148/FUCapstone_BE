namespace Identity.API.Payloads.Responses;

public sealed class UserResponseDTO
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }

    public string UserCode { get; set; }

    public string MajorId { get; set; }

    public string CampusId { get; set; }

    public string CapstoneId { get; set; }
    
    public string Email { get; set; }

}
