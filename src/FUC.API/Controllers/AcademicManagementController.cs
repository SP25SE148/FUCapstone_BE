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
    ICampusService _campusService,
    IMajorService _majorService,
    ICapstoneService _capstoneService,
    IMajorGroupService _majorGroupService) : ApiController
{
    #region Campus
    // ---- Campus Endpoints ----
    [HttpGet("campus")]
    public async Task<IActionResult> GetAllCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await _campusService.GetAllCampusAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("campus/active")]
    public async Task<IActionResult> GetAllActiveCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await _campusService.GetAllActiveCampusAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    
    [HttpPost("campus")]
    public async Task<IActionResult> CreateCampusAsync(CreateCampusRequest request)
    {
        OperationResult<string> result = await _campusService.CreateCampusAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("campus/{id}")]
    public async Task<IActionResult> GetCampusByIdAsync(string id)
    {
        OperationResult<CampusResponse> result = await _campusService.GetCampusByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpDelete("campus/{id}")]
    public async Task<IActionResult> DeleteCampusAsync(string id)
    {
        OperationResult result = await _campusService.DeleteCampusAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpPut("campus")]
    public async Task<IActionResult> UpdateCampusAsync([FromBody] UpdateCampusRequest request)
    {
       OperationResult<CampusResponse> result = await _campusService.UpdateCampusAsync(request);
       return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion
    
    #region Major
     // ---- Major Endpoints ----
    [HttpGet("major")]
    public async Task<IActionResult> GetAllMajorsAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await _majorService.GetAllMajorsAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("major/active")]
    public async Task<IActionResult> GetAllActiveMajorAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await _majorService.GetAllActiveMajorsAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }

    [HttpGet("major/by-major-group/{majorGroupId}")]
    public async Task<IActionResult> GetMajorsByMajorGroupIdAsync(string majorGroupId)
    {
        OperationResult<IEnumerable<MajorResponse>> result = await _majorService.GetMajorsByMajorGroupIdAsync(majorGroupId);
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    [HttpGet("major/{id}")]
    public async Task<IActionResult> GetMajorByIdAsync(string id)
    {
        OperationResult<MajorResponse> result = await _majorService.GetMajorByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("major")]
    public async Task<IActionResult> CreateMajorAsync(CreateMajorRequest request)
    {
        OperationResult<string> result = await _majorService.CreateMajorAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPut("major")]
    public async Task<IActionResult> UpdateMajorAsync([FromBody] UpdateMajorRequest request)
    {
        OperationResult<MajorResponse> result = await _majorService.UpdateMajorAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("major/{id}")]
    public async Task<IActionResult> DeleteMajorAsync(string id)
    {
        OperationResult result = await _majorService.DeleteMajorAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion

    #region MajorGroup
    
    // ---- MajorGroup Endpoints ----
    [HttpGet("majorgroup")]
    public async Task<IActionResult> GetAllMajorGroupsAsync()
    {
        OperationResult<IEnumerable<MajorGroupResponse>> result = await _majorGroupService.GetAllMajorGroupsAsync();
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("majorgroup/active")]
    public async Task<IActionResult> GetAllActiveMajorGroupAsync()
    {
        OperationResult<IEnumerable<MajorGroupResponse>> result = await _majorGroupService.GetAllActiveMajorGroupsAsync();
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    [HttpGet("majorgroup/{id}")]
    public async Task<IActionResult> GetMajorGroupByIdAsync(string id)
    {
        OperationResult<MajorGroupResponse> result = await _majorGroupService.GetMajorGroupByIdAsync(id);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("majorgroup")]
    public async Task<IActionResult> CreateMajorGroupAsync(CreateMajorGroupRequest request)
    {
        OperationResult<string> result = await _majorGroupService.CreateMajorGroupAsync(request);
        return !result.IsFailure ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPut("majorgroup")]
    public async Task<IActionResult> UpdateMajorGroupAsync([FromBody] UpdateMajorGroupRequest request)
    {
        OperationResult<MajorGroupResponse> result = await _majorGroupService.UpdateMajorGroupAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("majorgroup/{id}")]
    public async Task<IActionResult> DeleteMajorGroupAsync(string id)
    {
        OperationResult result = await _majorGroupService.DeleteMajorGroupAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion

    #region Capstone
    // ---- Capstone Endpoints ----
    [HttpGet("capstone")]
    public async Task<IActionResult> GetAllCapstonesAsync()
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await _capstoneService.GetAllCapstonesAsync();
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("capstone/active")]
    public async Task<IActionResult> GetAllActiveCapstoneAsync()
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await _capstoneService.GetAllActiveCapstonesAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("capstone/by-major/{majorId}")]
    public async Task<IActionResult> GetCapstoneByMajorIdAsync(string majorId)
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await _capstoneService.GetCapstonesByMajorIdAsync(majorId);
        return !result.IsFailure
            ? Ok(result.Value)
            : HandleFailure(result);
    }
    
    [HttpGet("capstone/{id}")]
    public async Task<IActionResult> GetCapstoneByIdAsync(string id)
    {
        OperationResult<CapstoneResponse> result = await _capstoneService.GetCapstoneByIdAsync(id);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("capstone")]
    public async Task<IActionResult> CreateCapstoneAsync(CreateCapstoneRequest request)
    {
        OperationResult<string> result = await _capstoneService.CreateCapstoneAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("capstone")]
    public async Task<IActionResult> UpdateCapstoneAsync([FromBody] UpdateCapstoneRequest request)
    {
        OperationResult<CapstoneResponse> result = await _capstoneService.UpdateCapstoneAsync(request);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("capstone/{id}")]
    public async Task<IActionResult> DeleteCapstoneAsync(string id)
    {
        OperationResult result = await _capstoneService.DeleteCapstoneAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion
}
