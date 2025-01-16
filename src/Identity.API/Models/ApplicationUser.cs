using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public int MajorId { get; set; }

    public int CampusId { get; set; }
}
