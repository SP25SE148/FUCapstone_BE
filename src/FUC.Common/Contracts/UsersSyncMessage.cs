namespace FUC.Common.Contracts;
public class UsersSyncMessage
{
    public int AttempTime { get; set; }

    public string UserType { get; set; }

    public IEnumerable<UserSync> UsersSync { get; set; }
}
