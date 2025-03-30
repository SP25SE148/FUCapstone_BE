namespace FUC.Data.Enums;

public enum GroupStatus
{
    Pending,
    InProgress,
    Completed,
    InCompleted, // use when supervisor/mentor decide to all member of group can not defend capstone at attempt 1 and 2 
    Deleted
}
