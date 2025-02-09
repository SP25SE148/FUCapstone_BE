using System.Security.Claims;
using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public class GroupController(IGroupService groupService) : ApiController
{
    [Authorize(Roles = nameof(UserRoles.Student))]
    [HttpPost]
    public async Task<IActionResult> CreateGroupAsync(CreateGroupRequest request)
    {
        OperationResult<Guid> result = await groupService.CreateGroupAsync(request,User.FindFirst(ClaimTypes.GivenName)!.Value);
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
