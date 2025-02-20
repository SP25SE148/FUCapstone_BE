using System.Security.Claims;
using FUC.Common.Abstractions;

namespace Identity.API.Infrastuctures;
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id => httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    public string Name => httpContextAccessor.HttpContext!.User.FindFirst("name")!.Value;

    public string Email => httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!.Value;

    public string UserCode => httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.GivenName)!.Value;

    public string MajorId => httpContextAccessor.HttpContext!.User.FindFirst("MajorId")!.Value;

    public string CapstoneId => httpContextAccessor.HttpContext!.User.FindFirst("CapstoneId")!.Value;

    public string CampusId => httpContextAccessor.HttpContext!.User.FindFirst("CampusId")!.Value;
    public string Role => httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value;
}
