using System.ComponentModel.DataAnnotations;

namespace Identity.API.Payloads.Requests;

public class StudentDto
{
    [Required]
    [RegularExpression(@"^[A-Z]{2}\d{6}$", ErrorMessage = "Invalid student code format.")]
    public string StudentCode { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]  
    public string UserName { get; set; }

    [Required]
    public string MajorId { get; set; }

    [Required]
    public string CapstoneId { get; set; }

    [Required]  
    public string CampusId { get; set; }
}
