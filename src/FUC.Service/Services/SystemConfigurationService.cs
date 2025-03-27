using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using Microsoft.Extensions.Options;

namespace FUC.Service.Services;

public class SystemConfigurationService : ISystemConfigurationService
{
    private readonly SystemConfiguration _config;

    public SystemConfigurationService(IOptions<SystemConfiguration> options)
    {
        _config = options.Value;
    }

    public SystemConfiguration GetSystemConfiguration()
    {
        return _config;
    }

    public void UpdateMaxTopicsForCoSupervisors(int value)
    {
        _config.MaxTopicsForCoSupervisors = value;
    }

    public void UpdateMaxTopicAppraisalsForTopic(int value)
    {
        _config.MaxTopicAppraisalsForTopic = value;
    }

    public void UpdateExpirationTopicRequestDuration(double value)
    {
        _config.ExpirationTopicRequestDuration = value;
    }

    public void UpdateExpirationTeamUpDuration(double value)
    {
        _config.ExpirationTeamUpDuration = value;
    }

    public void UpdateMaxAttemptTimesToDefendCapstone(int value)
    {
        _config.MaxAttemptTimesToDefendCapstone = value;
    }

    public void UpdateMaxAttemptTimesToReviewTopic(int value)
    {
        _config.MaxAttemptTimesToReviewTopic = value;
    }
}

