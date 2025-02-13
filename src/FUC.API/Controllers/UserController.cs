using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.StudentDTO;
using FUC.Service.DTOs.SupervisorDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public sealed class UserController(IStudentService studentService, ISupervisorService supervisorService) : ApiController
{

    [HttpGet("get-all-student")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}, {UserRoles.Admin}, {UserRoles.Manager}")]
    public async Task<IActionResult> GetAllStudentAsync()
    {
        OperationResult<IEnumerable<StudentResponseDTO>> result = await studentService.GetAllStudentAsync();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }
    
    [HttpGet("get-all-supervisor")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}, {UserRoles.Admin}, {UserRoles.Manager}")]
    public async Task<IActionResult> GetAllSupervisorAsync()
    {
        OperationResult<IEnumerable<SupervisorResponseDTO>> result = await supervisorService.GetAllSupervisorAsync();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }
}
