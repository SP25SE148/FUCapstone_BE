using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class SupervisorDto
{
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string MajorId { get; set; }
}
