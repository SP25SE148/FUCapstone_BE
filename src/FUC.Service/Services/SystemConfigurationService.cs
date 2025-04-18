using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FUC.Service.Services;

public class SystemConfigurationService : ISystemConfigurationService
{
    private readonly SystemConfiguration _config;
    private readonly CapstoneService _capstoneService;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Student> _studentRepository;

    public SystemConfigurationService(IOptions<SystemConfiguration> options, 
        CapstoneService capstoneService,
        ICurrentUser currentUser,
        IRepository<Student> studentRepository)
    {
        _config = options.Value;
        _capstoneService = capstoneService; 
        _currentUser = currentUser;
        _studentRepository = studentRepository;
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

    public void UpdateSemanticTopicThroughSemesters(int value)
    {
        _config.SemanticTopicThroughSemesters = value;
    }

    public async Task<OperationResult> UpdateMininumTopicsPerCapstoneInEachCampus()
    {
        var campus = _currentUser.CampusId;

        var capstones = await _capstoneService.GetAllCapstonesAsync();

        var mininumTopicsPerCapstone = new Dictionary<string, double>();  

        foreach (var capstone in capstones.Value)
        {
            var students = await _studentRepository.GetQueryable().Where(x => x.CapstoneId == capstone.Id).CountAsync();

            var topics = students / capstone.MinMember;

            mininumTopicsPerCapstone[capstone.Id] = Math.Ceiling(topics + topics * 0.1);
        }

        _config.MininumTopicsPerCapstoneInEachCampus[campus] = mininumTopicsPerCapstone;

        return OperationResult.Success();
    }

    public async Task<Dictionary<string, double>> GetMinimumTopicsByMajorId() 
    {
        var capstonesByMajorResult = await _capstoneService.GetCapstonesByMajorIdAsync(_currentUser.MajorId);

        if (capstonesByMajorResult.IsFailure) throw new InvalidOperationException();

        var capstoneIds = capstonesByMajorResult.Value.Select(x => x.Id).ToList();

        return _config.MininumTopicsPerCapstoneInEachCampus[_currentUser.CampusId]
            .Where(x => capstoneIds.Contains(x.Key))
            .ToDictionary();
    } 
}

