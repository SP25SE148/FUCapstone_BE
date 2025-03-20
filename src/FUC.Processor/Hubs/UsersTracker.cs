using System.Collections.Concurrent;

namespace FUC.Processor.Hubs;

public class UsersTracker
{
    private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new();

    public Task<bool> UserConnected(string userCode, string connectionId)
    {
        bool isOnline = false;

        if (OnlineUsers.ContainsKey(userCode))
        {
            OnlineUsers[userCode].Add(connectionId);
        }
        else
        {
            OnlineUsers.TryAdd(userCode, new List<string> { connectionId });
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string userCode, string connectionId)
    {
        bool isOffline = false;

        if (!OnlineUsers.ContainsKey(userCode))
            return Task.FromResult(isOffline);

        OnlineUsers[userCode].Remove(connectionId);

        if (OnlineUsers[userCode].Count == 0)
        {
            OnlineUsers.TryRemove(userCode, out _);
            isOffline = true;
        }

        return Task.FromResult(isOffline);
    }

    public Task<List<string>> GetConnectionForUser(string userCode)
    {
        OnlineUsers.TryGetValue(userCode, out List<string>? value);

        return Task.FromResult(value ?? new List<string>());
    }
}
