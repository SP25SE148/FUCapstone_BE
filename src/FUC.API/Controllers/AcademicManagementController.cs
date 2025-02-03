using FUC.API.Abstractions;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.MajorDTO;
using FUC.Service.DTOs.MajorGroupDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize(Roles = "SuperAdmin")]
public sealed class AcademicManagementController(
    ICampusService campusService,
    IMajorService majorService,
    ICapstoneService capstoneService,
    IMajorGroupService majorGroupService) : ApiController
{
    #region Campus
    // ---- Campus Endpoints ----
    [HttpGet("campus")]
    public async Task<IActionResult> GetAllCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await campusService.GetAllCampusAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("campus/active")]
    public async Task<IActionResult> GetAllActiveCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await campusService.GetAllActiveCampusAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    
    [HttpPost("campus")]
    public async Task<IActionResult> CreateCampusAsync(CreateCampusRequest request)
    {
        OperationResult<string> result = await campusService.CreateCampusAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("campus/{id}")]
    public async Task<IActionResult> GetCampusByIdAsync(string id)
    {
        OperationResult<CampusResponse> result = await campusService.GetCampusByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpDelete("campus/{id}")]
    public async Task<IActionResult> DeleteCampusAsync(string id)
    {
        OperationResult result = await campusService.DeleteCampusAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpPut("campus")]
    public async Task<IActionResult> UpdateCampusAsync([FromBody] UpdateCampusRequest request)
    {
       OperationResult<CampusResponse> result = await campusService.UpdateCampusAsync(request);
       return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion
    
    #region Major
     // ---- Major Endpoints ----
    [HttpGet("major")]
    public async Task<IActionResult> GetAllMajorsAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await majorService.GetAllMajorsAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("major/active")]
    public async Task<IActionResult> GetAllActiveMajorAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await majorService.GetAllActiveMajorsAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("major/{id}")]
    public async Task<IActionResult> GetMajorByIdAsync(string id)
    {
        OperationResult<MajorResponse> result = await majorService.GetMajorByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("major")]
    public async Task<IActionResult> CreateMajorAsync(CreateMajorRequest request)
    {
        OperationResult<string> result = await majorService.CreateMajorAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPut("major")]
    public async Task<IActionResult> UpdateMajorAsync([FromBody] UpdateMajorRequest request)
    {
        OperationResult<MajorResponse> result = await majorService.UpdateMajorAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("major/{id}")]
    public async Task<IActionResult> DeleteMajorAsync(string id)
    {
        OperationResult result = await majorService.DeleteMajorAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion

    #region MajorGroup
    
    // ---- MajorGroup Endpoints ----
    [HttpGet("majorgroup")]
    public async Task<IActionResult> GetAllMajorGroupsAsync()
    {
        OperationResult<IEnumerable<MajorGroupResponse>> result = await majorGroupService.GetAllMajorGroupsAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("majorgroup/active")]
    public async Task<IActionResult> GetAllActiveMajorGroupAsync()
    {
        OperationResult<IEnumerable<MajorGroupResponse>> result = await majorGroupService.GetAllActiveMajorGroupsAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    [HttpGet("majorgroup/{id}")]
    public async Task<IActionResult> GetMajorGroupByIdAsync(string id)
    {
        OperationResult<MajorGroupResponse> result = await majorGroupService.GetMajorGroupByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("majorgroup")]
    public async Task<IActionResult> CreateMajorGroupAsync(CreateMajorGroupRequest request)
    {
        OperationResult<string> result = await majorGroupService.CreateMajorGroupAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPut("majorgroup")]
    public async Task<IActionResult> UpdateMajorGroupAsync([FromBody] UpdateMajorGroupRequest request)
    {
        OperationResult<MajorGroupResponse> result = await majorGroupService.UpdateMajorGroupAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("majorgroup/{id}")]
    public async Task<IActionResult> DeleteMajorGroupAsync(string id)
    {
        OperationResult result = await majorGroupService.DeleteMajorGroupAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion

    #region Capstone
    // ---- Capstone Endpoints ----
    [HttpGet("capstone")]
    public async Task<IActionResult> GetAllCapstonesAsync()
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await capstoneService.GetAllCapstonesAsync();
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("capstone/active")]
    public async Task<IActionResult> GetAllActiveCapstoneAsync()
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await capstoneService.GetAllActiveCapstonesAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }
    [HttpGet("capstone/{id}")]
    public async Task<IActionResult> GetCapstoneByIdAsync(string id)
    {
        OperationResult<CapstoneResponse> result = await capstoneService.GetCapstoneByIdAsync(id);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("capstone")]
    public async Task<IActionResult> CreateCapstoneAsync(CreateCapstoneRequest request)
    {
        OperationResult<string> result = await capstoneService.CreateCapstoneAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("capstone")]
    public async Task<IActionResult> UpdateCapstoneAsync([FromBody] UpdateCapstoneRequest request)
    {
        OperationResult<CapstoneResponse> result = await capstoneService.UpdateCapstoneAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("capstone/{id}")]
    public async Task<IActionResult> DeleteCapstoneAsync(string id)
    {
        OperationResult result = await capstoneService.DeleteCapstoneAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion
}
