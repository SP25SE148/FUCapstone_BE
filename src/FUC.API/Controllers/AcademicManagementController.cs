using FUC.API.Abstractions;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.MajorDTO;
using FUC.Service.DTOs.MajorGroupDTO;
using FUC.Service.DTOs.SemesterDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public sealed class AcademicManagementController(
    ICampusService campusService,
    IMajorService majorService,
    ICapstoneService capstoneService,
    IMajorGroupService majorGroupService,
    ISemesterService semesterService,
    IArchiveDataApplicationService archiveDataApplicationService,
    ICurrentUser currentUser) : ApiController
{
    #region Campus
    // ---- Campus Endpoints ----
    [HttpGet("campus")]
    public async Task<IActionResult> GetAllCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await campusService.GetAllCampusAsync();
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("campus/active")]
    public async Task<IActionResult> GetAllActiveCampusAsync()
    {
        OperationResult<IEnumerable<CampusResponse>> result = await campusService.GetAllActiveCampusAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }
    
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPost("campus")]
    public async Task<IActionResult> CreateCampusAsync(CreateCampusRequest request)
    {
        OperationResult<string> result = await campusService.CreateCampusAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("campus/{id}")]
    public async Task<IActionResult> GetCampusByIdAsync(string id)
    {
        OperationResult<CampusResponse> result = await campusService.GetCampusByIdAsync(id);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }
    
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpDelete("campus/{id}")]
    public async Task<IActionResult> DeleteCampusAsync(string id)
    {
        OperationResult result = await campusService.DeleteCampusAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPut("campus")]
    public async Task<IActionResult> UpdateCampusAsync([FromBody] UpdateCampusRequest request)
    {
       OperationResult<CampusResponse> result = await campusService.UpdateCampusAsync(request);
       return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }
    #endregion
    
    #region Major
     // ---- Major Endpoints ----
    [HttpGet("major")]
    public async Task<IActionResult> GetAllMajorsAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await majorService.GetAllMajorsAsync();
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("major/active")]
    public async Task<IActionResult> GetAllActiveMajorAsync()
    {
        OperationResult<IEnumerable<MajorResponse>> result = await majorService.GetAllActiveMajorsAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("major/by-major-group/{majorGroupId}")]
    public async Task<IActionResult> GetMajorsByMajorGroupIdAsync(string majorGroupId)
    {
        OperationResult<IEnumerable<MajorResponse>> result = await majorService.GetMajorsByMajorGroupIdAsync(majorGroupId);
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }
    
    [HttpGet("major/{id}")]
    public async Task<IActionResult> GetMajorByIdAsync(string id)
    {
        OperationResult<MajorResponse> result = await majorService.GetMajorByIdAsync(id);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }
    
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPost("major")]
    public async Task<IActionResult> CreateMajorAsync(CreateMajorRequest request)
    {
        OperationResult<string> result = await majorService.CreateMajorAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPut("major")]
    public async Task<IActionResult> UpdateMajorAsync([FromBody] UpdateMajorRequest request)
    {
        OperationResult<MajorResponse> result = await majorService.UpdateMajorAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
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
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("majorgroup/active")]
    public async Task<IActionResult> GetAllActiveMajorGroupAsync()
    {
        OperationResult<IEnumerable<MajorGroupResponse>> result = await majorGroupService.GetAllActiveMajorGroupsAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }
    [HttpGet("majorgroup/{id}")]
    public async Task<IActionResult> GetMajorGroupByIdAsync(string id)
    {
        OperationResult<MajorGroupResponse> result = await majorGroupService.GetMajorGroupByIdAsync(id);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPost("majorgroup")]
    public async Task<IActionResult> CreateMajorGroupAsync(CreateMajorGroupRequest request)
    {
        OperationResult<string> result = await majorGroupService.CreateMajorGroupAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPut("majorgroup")]
    public async Task<IActionResult> UpdateMajorGroupAsync([FromBody] UpdateMajorGroupRequest request)
    {
        OperationResult<MajorGroupResponse> result = await majorGroupService.UpdateMajorGroupAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
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

    [HttpGet("capstone/by-major/{majorId}")]
    public async Task<IActionResult> GetCapstoneByMajorIdAsync(string majorId)
    {
        OperationResult<IEnumerable<CapstoneResponse>> result = await capstoneService.GetCapstonesByMajorIdAsync(majorId);
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

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPost("capstone")]
    public async Task<IActionResult> CreateCapstoneAsync(CreateCapstoneRequest request)
    {
        OperationResult<string> result = await capstoneService.CreateCapstoneAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPut("capstone")]
    public async Task<IActionResult> UpdateCapstoneAsync([FromBody] UpdateCapstoneRequest request)
    {
        OperationResult<CapstoneResponse> result = await capstoneService.UpdateCapstoneAsync(request);
        return !result.IsFailure ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpDelete("capstone/{id}")]
    public async Task<IActionResult> DeleteCapstoneAsync(string id)
    {
        OperationResult result = await capstoneService.DeleteCapstoneAsync(id);
        return !result.IsFailure ? NoContent() : HandleFailure(result);
    }
    #endregion

    #region Semester

    [HttpGet("semester")]
    public async Task<IActionResult> GetAllSemestersAsync()
    {
        OperationResult<IEnumerable<SemesterResponse>> semesters = await semesterService.GetSemestersAsync();
        return !semesters.IsFailure
            ? Ok(semesters)
            : HandleFailure(semesters);
    }

    [HttpGet("semester/active")]
    public async Task<IActionResult> GetAllActiveSemestersAsync()
    {
        OperationResult<IEnumerable<SemesterResponse>> semesters = await semesterService.GetAllActiveSemestersAsync();
        return !semesters.IsFailure
            ? Ok(semesters)
            : HandleFailure(semesters);
    }

    [HttpGet("semester/{semesterId}", Name = "GetSemesterById")]
    public async Task<IActionResult> GetSemesterByIdAsync(string semesterId)
    {
        OperationResult<SemesterResponse> result = await semesterService.GetSemesterByIdAsync(semesterId);
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }
    
    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPost("semester")]
    public async Task<IActionResult> CreateSemesterAsync(CreateSemesterRequest request)
    {
        OperationResult<string> result = await semesterService.CreateSemesterAsync(request);
        return !result.IsFailure
            ? CreatedAtRoute(
                "GetSemesterById",
                new {semesterId = result.Value},
                new {Id = result.Value}
                )
            : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpPut("semester")]
    public async Task<IActionResult> UpdateSemesterAsync(UpdateSemesterRequest request)
    {
        OperationResult<SemesterResponse> result = await semesterService.UpdateSemesterAsync(request);
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.SuperAdmin))]
    [HttpDelete("semester/{semesterId}")]
    public async Task<IActionResult> DeleteSemesterAsync(string semesterId)
    {
        OperationResult result = await semesterService.DeleteSemesterAsync(semesterId);
        return !result.IsFailure
            ? NoContent()
            : HandleFailure(result);
    }
    #endregion

    #region Archive 

    [HttpPost("archive")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> ArchiveData()
    {
        var result = await archiveDataApplicationService.ArchiveDataCompletedStudents(default);

        return result.IsSuccess
           ? File(result.Value.Content,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               result.Value.FileName)
           : HandleFailure(result);
    }

    #endregion Archive

    #region Dashboard

    [HttpGet("dashboard")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetDashboardAsync()
    {
        switch (currentUser.Role)
        {
            case UserRoles.SuperAdmin:
                var superAdmin = await archiveDataApplicationService.PresentSuperAdminDashBoard(default);

                return superAdmin.IsSuccess ? Ok(superAdmin) : HandleFailure(superAdmin);   

            case UserRoles.Admin:
                var admin = await archiveDataApplicationService.PresentAdminDashBoard(default);

                return admin.IsSuccess ? Ok(admin) : HandleFailure(admin);

            case UserRoles.Manager:
                var manager = await archiveDataApplicationService.PresentManagerDashBoard(default);

                return manager.IsSuccess ? Ok(manager) : HandleFailure(manager);

            default:
                throw new InvalidOperationException();
        }
    }
    #endregion Dashboard
}
