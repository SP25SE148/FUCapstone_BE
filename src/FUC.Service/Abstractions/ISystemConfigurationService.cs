﻿using FUC.Common.Shared;
using FUC.Service.Extensions.Options;

namespace FUC.Service.Abstractions;

public interface ISystemConfigurationService
{
    SystemConfiguration GetSystemConfiguration();
    void UpdateMaxTopicsForCoSupervisors(int value);
    void UpdateMaxTopicAppraisalsForTopic(int value);
    void UpdateExpirationTopicRequestDuration(double value);
    void UpdateExpirationTeamUpDuration(double value);
    void UpdateMaxAttemptTimesToDefendCapstone(int value);
    void UpdateMaxAttemptTimesToReviewTopic(int value);
    void UpdateSemanticTopicThroughSemesters(int value);
    void UpdateTimeConfigurationRemindInDaysBeforeDueDate(int value);
    void UpdateProjectProgressRemindInDaysBeforeDueDate(int value);
    void UpdateMinimumPercentageOfStudentsDefend(double value);
    Task<OperationResult> UpdateMininumTopicsPerCapstoneInEachCampus();
    Task<Dictionary<string, double>> GetMinimumTopicsByMajorId();
    double GetGetMaxTopicsOfCapstone(string capstoneId);
}
