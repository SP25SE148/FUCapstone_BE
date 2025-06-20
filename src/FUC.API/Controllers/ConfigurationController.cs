﻿using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.ConfigDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public sealed class ConfigurationController : ApiController
{
    private readonly ISystemConfigurationService _systemConfigService;
    private readonly ITimeConfigurationService _timeConfigurationService;

    public ConfigurationController(ISystemConfigurationService systemConfigService,
        ITimeConfigurationService timeConfigurationService)
    {
        _systemConfigService = systemConfigService;
        _timeConfigurationService = timeConfigurationService;
    }

    #region system_config

    [HttpGet("system")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult GetSystemConfiguration()
    {
        var config = _systemConfigService.GetSystemConfiguration();
        return Ok(OperationResult.Success(config));
    }

    [HttpPatch("system/MaxTopicsForCoSupervisors")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateMaxTopicsForCoSupervisors([FromBody] int value)
    {
        _systemConfigService.UpdateMaxTopicsForCoSupervisors(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/MaxTopicAppraisalsForTopic")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateMaxTopicAppraisalsForTopic([FromBody] int value)
    {
        _systemConfigService.UpdateMaxTopicAppraisalsForTopic(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/ExpirationTopicRequestDuration")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateExpirationTopicRequestDuration([FromBody] double value)
    {
        _systemConfigService.UpdateExpirationTopicRequestDuration(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/ExpirationTeamUpDuration")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateExpirationTeamUpDuration([FromBody] double value)
    {
        _systemConfigService.UpdateExpirationTeamUpDuration(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/MaxAttemptTimesToDefendCapstone")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateMaxAttemptTimesToDefendCapstone([FromBody] int value)
    {
        _systemConfigService.UpdateMaxAttemptTimesToDefendCapstone(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/MaxAttemptTimesToReviewTopic")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateMaxAttemptTimesToReviewTopic([FromBody] int value)
    {
        _systemConfigService.UpdateMaxAttemptTimesToReviewTopic(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/SemanticTopicThroughSemesters")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateSemanticTopicThroughSemesters([FromBody] int value)
    {
        _systemConfigService.UpdateSemanticTopicThroughSemesters(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/ProjectProgressRemindInDaysBeforeDueDate")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateProjectProgressRemindInDaysBeforeDueDate([FromBody] int value)
    {
        _systemConfigService.UpdateProjectProgressRemindInDaysBeforeDueDate(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/TimeConfigurationRemindInDaysBeforeDueDate")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateTimeConfigurationRemindInDaysBeforeDueDate([FromBody] int value)
    {
        _systemConfigService.UpdateTimeConfigurationRemindInDaysBeforeDueDate(value);
        return Ok(OperationResult.Success());
    }

    [HttpPatch("system/MinimumPercentageOfStudentsDefend")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public IActionResult UpdateMinimumPercentageOfStudentsDefend([FromBody] double value)
    {
        _systemConfigService.UpdateMinimumPercentageOfStudentsDefend(value);
        return Ok(OperationResult.Success());
    }

    [HttpPost("estimate/minimum-topics")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> EstimateMinimumTopics()
    {
        var result = await _systemConfigService.UpdateMininumTopicsPerCapstoneInEachCampus();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    #endregion system_config

    #region time_config

    [HttpGet("time")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> GetTimeConfigurations()
    {
        var config = await _timeConfigurationService.GetTimeConfigurations();

        return config.IsSuccess ? Ok(config) : HandleFailure(config);
    }

    [HttpPost("time")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> CreateTimeConfiguration(CreateTimeConfigurationRequest request)
    {
        var config = await _timeConfigurationService.CreateTimeConfiguration(request, default);

        return config.IsSuccess ? Ok(config) : HandleFailure(config);
    }

    [HttpPut("time")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> UpdateTimeConfiguration(UpdateTimeConfigurationRequest request)
    {
        var config = await _timeConfigurationService.UpdateTimeConfiguration(request, default);

        return config.IsSuccess ? Ok(config) : HandleFailure(config);
    }

    [HttpGet("time-by-semester-id/{semesterId}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> GetTimeConfigurationsBySemesterId(string semesterId)
    {
        var timeConfig = await _timeConfigurationService.GetTimeConfigurationBySemesterId(semesterId);
        return timeConfig.IsSuccess ? Ok(timeConfig) : HandleFailure(timeConfig);
    }

    #endregion time_config
}
