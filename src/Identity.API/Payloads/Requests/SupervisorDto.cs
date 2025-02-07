using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class SupervisorDto
{
    [EmailAddress]
    public string Email { get; set; }

    public string UserName { get; set; }

    [Required]
    public string MajorId { get; set; }

    [Required]  
    public string CampusId { get; set; }
}
