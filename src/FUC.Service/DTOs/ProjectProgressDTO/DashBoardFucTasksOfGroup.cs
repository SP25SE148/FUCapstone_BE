namespace FUC.Service.DTOs.ProjectProgressDTO;

public class DashBoardFucTasksOfGroup
{
    public required DashBoardFucTasksDetail DashBoardFucTask { get; set; }
    public List<DashBoardFucTasksStudent> DashBoardFucTasksStudents { get; set; } = new();
}

public class DashBoardFucTasksStudent 
{
    public required DashBoardFucTasksDetail DashBoardFucTask { get; set; }
    public required string StudentId { get; set; }
}

public class DashBoardFucTasksDetail
{
    public int TotalTasks { get; set; }
    public int TotalInprogressTasks { get; set; }
    public int TotalToDoTasks { get; set; }
    public int TotalDoneTasks { get; set; }
    public int TotalExpiredTasks { get; set; }
}
