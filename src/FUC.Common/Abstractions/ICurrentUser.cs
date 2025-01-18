namespace FUC.Common.Abstractions;
public interface ICurrentUser
{
    public string Id { get; }
    public string Name { get; }
    public string Email { get; }
}

