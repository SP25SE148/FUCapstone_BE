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
    private readonly IRepository<Capstone> _capstoneRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Student> _studentRepository;

    public SystemConfigurationService(IOptions<SystemConfiguration> options,
        IRepository<Capstone> capstoneRepository,
        ICurrentUser currentUser,
        IRepository<Student> studentRepository)
    {
        _config = options.Value;
        _capstoneRepository = capstoneRepository;
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

    public void UpdateTimeConfigurationRemindInDaysBeforeDueDate(int value)
    {
        _config.TimeConfigurationRemindInDaysBeforeDueDate = value;
    }

    public void UpdateProjectProgressRemindInDaysBeforeDueDate(int value)
    {
        _config.ProjectProgressRemindInDaysBeforeDueDate = value;
    }

    public async Task<OperationResult> UpdateMininumTopicsPerCapstoneInEachCampus()
    {
        try
        {
            var campus = _currentUser.CampusId;

            var capstones = await _capstoneRepository.GetAllAsync();

            var mininumTopicsPerCapstone = new Dictionary<string, double>();

            foreach (var capstone in capstones)
            {
                var students = await _studentRepository.GetQueryable()
                    .Where(x => x.CapstoneId == capstone.Id)
                    .CountAsync();

                var topics = students / (double)capstone.MinMember;

                mininumTopicsPerCapstone[capstone.Id] = Math.Ceiling(topics + topics * 0.1);
            }

            var config = _config.MininumTopicsPerCapstoneInEachCampus;

            config[campus] = mininumTopicsPerCapstone;

            return OperationResult.Success();
        }
        catch (Exception)
        {
            return OperationResult.Failure(new Error("SystemConfig.Error", "Fail to estimate."));
        }
    }

    public async Task<Dictionary<string, double>> GetMinimumTopicsByMajorId() 
    {
        var capstonesByMajor = await _capstoneRepository
        .FindAsync(
            x => _currentUser.MajorId == "all" || x.MajorId == _currentUser.MajorId,
            include: null,
            orderBy: null,
            selector: x => x.Id
        );

        if (capstonesByMajor.Count == 0)
            throw new InvalidOperationException("No capstones found for the given major.");

        var configForCampus = _config.MininumTopicsPerCapstoneInEachCampus
            .GetValueOrDefault(_currentUser.CampusId);

        if (configForCampus == null)
            return new Dictionary<string, double>(); // or throw if you prefer

        var result = configForCampus
            .Where(x => x.Key != null && capstonesByMajor.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value);

        return result;
    }

    public double GetMinimumTopicsByMajorId(string majorId)
    {
        if (!_config.MininumTopicsPerCapstoneInEachCampus.TryGetValue(_currentUser.CampusId, out Dictionary<string, double>? value))
            return 0;

        var result = value.TryGetValue(majorId, out double maxTopics);

        return result ? maxTopics : 0;  
    }
}

