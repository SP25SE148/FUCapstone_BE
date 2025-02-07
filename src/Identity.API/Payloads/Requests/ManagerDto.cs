using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class ManagerDto
{
    [EmailAddress]
    public string Email { get; set; }

    public string UserName { get; set; }

    [Required]  
    public string CampusId { get; set; }
}
