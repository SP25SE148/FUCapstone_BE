using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }

    public string UserCode { get; set; }

    public string MajorId { get; set; }

    public string CampusId { get; set; }

    public string CapstoneId { get; set; }
}
