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

    [HttpGet("get-by-semester-id/{semesterId}")]
    public async Task<IActionResult> GetGroupsBySemesterId(string semesterId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupBySemesterIdAsync(semesterId);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    [HttpGet("get-by-major-id/{majorId}")]
    public async Task<IActionResult> GetGroupsByMajorId(string majorId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupByMajorIdAsync(majorId);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    [HttpGet("get-by-capstone-id/{capstoneId}")]
    public async Task<IActionResult> GetGroupsByCampusId(string capstoneId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupByCapstoneIdAsync(capstoneId);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupByIdAsync(Guid id)
    {
        OperationResult<GroupResponse> result = await groupService.GetGroupByIdAsync(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("get-by-campus-id/{campusId}")]
    public async Task<IActionResult> GetGroupByCampusIdAsync(string campusId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupByCampusIdAsync(campusId);
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleFailure(result);
    }
}
