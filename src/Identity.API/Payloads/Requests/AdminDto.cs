using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class AdminDto
{
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string CampusId { get; set; }
}
