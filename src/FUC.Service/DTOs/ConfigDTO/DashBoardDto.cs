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
}

public record class ManagerDashBoardDto : DashBoardDto
{
    public Dictionary<string, int> TopicsInEachStatus { get; set; }
}
