using System.Security.Claims;
using FUC.Common.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.Infrastructure;
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id => httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    public string Name => httpContextAccessor.HttpContext.User.FindFirst("name")!.Value;

    public string Email => httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;

    public string UserCode => httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GivenName)!.Value;
}
