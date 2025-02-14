using System.Collections.Concurrent;

namespace FUC.Processor.Hubs;

public class UsersTracker
{
    private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new ConcurrentDictionary<string, List<string>>();

    public Task<bool> UserConnected(string email, string connectionId)
    {
        bool isOnline = false;

        if (OnlineUsers.ContainsKey(email))
        {
            OnlineUsers[email].Add(connectionId);
        }
        else
        {
            OnlineUsers.TryAdd(email, new List<string> { connectionId });
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string email, string connectionId)
    {
        bool isOffline = false;

        if (!OnlineUsers.ContainsKey(email))
            return Task.FromResult(isOffline);

        OnlineUsers[email].Remove(connectionId);

        if (OnlineUsers[email].Count == 0)
        {
            OnlineUsers.TryRemove(email, out _);
            isOffline = true;
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;

        onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();

        return Task.FromResult(onlineUsers);
    }

    // Get all the connectionId of user -> because when we want notify the message -> we should do it for all Connection
    public Task<List<string>> GetConnectionForUser(string email)
    {
        // get all the values by key -> using method GetValueOrDefault()
        OnlineUsers.TryGetValue(email, out List<string>? value);

        return Task.FromResult(value ?? new List<string>());
    }
}
