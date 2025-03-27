using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public sealed class ConfigurationController : ApiController
{
    private readonly ISystemConfigurationService _systemConfigService;

    public ConfigurationController(ISystemConfigurationService systemConfigService)
    {
        _systemConfigService = systemConfigService;
    }

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
}
