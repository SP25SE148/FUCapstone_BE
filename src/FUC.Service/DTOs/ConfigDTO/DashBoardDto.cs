using FUC.Data.Enums;

namespace FUC.Service.DTOs.ConfigDTO;

public record class DashBoardDto
{
    public int Students { get; set; }
    public int Supervisors { get; set; }
    public int Topics { get; set; }
    public int Groups { get; set; }
}

public record class SuperAdminDashBoardDto : DashBoardDto
{
    public Dictionary<string, DashBoardDto> DataPerCampus { get; set; }
    public int Capstones { get; set; }
}

public record class AdminDashBoardDto : DashBoardDto
{
    public Dictionary<string, int> SupervisorsInEachMajor { get; set; }
    public Dictionary<string, int> TopicsInEachCapstone { get; set; }
    public Dictionary<string, double> MaxTopicsOfCapstoneEachMajor { get; set; }
}

    public record class ManagerDashBoardDto : DashBoardDto
{
    public Dictionary<string, int> TopicsInEachStatus { get; set; }
    public double AverageGroupSize { get; set; }
    public double TaskCompletionRate { get; set; }
    public int OverdueTaskCount { get; set; }
    public GroupTaskMetrics? BestPerformingGroup { get; set; }
    public GroupTaskMetrics? WorstPerformingGroup { get; set; }
    public Dictionary<string, int> TopicsPerSupervisor { get; set; }
    public double MaxTopicsOfCapstone { get; set; }
}

public class SupervisorDashBoardDto
{
    public Dictionary<string, GroupTaskMetrics> GroupTaskMetrics { get; set; }
    public Dictionary<string, double?>? CompletionTaskRatios { get; set; }
    public Dictionary<string, double?>? OverdueTaskRatios { get; set; }
    public GroupTaskMetrics? GroupWithHighestCompletion { get; set; }
    public GroupTaskMetrics? GroupWithLowestOverdue { get; set; }
    public Dictionary<string, double> AverageTaskDurations { get; set; }
    public Dictionary<string, Dictionary<Priority, int>> TaskPriorityDistributions { get; set; }
    public Dictionary<string, double> StudentContributions { get; set; }
    public Dictionary<string, double> MaxTopicsOfCapstoneEachMajor { get; set; }
}

public class GroupTaskMetrics
{
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public double AverageTaskDuration { get; set; }
    public Dictionary<Priority, int>? PriorityDistribution { get; set; }
    public double? CompletionTaskRatio { get; set; }
    public double? OverdueTaskRatio { get; set; }
}
