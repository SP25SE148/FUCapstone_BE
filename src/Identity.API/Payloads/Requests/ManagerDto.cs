using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class ManagerDto
{
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string CapstoneId { get; set; }
}
