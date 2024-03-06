using System.Collections.Concurrent;
using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public class GeneratedUsersPool : IUsersPool
{
    private readonly ConcurrentDictionary<string, List<GetUserResponse>> _roleToUserIds = new();
    
    public void SetUsers(string role, List<GetUserResponse> users)
    {
        _roleToUserIds.AddOrUpdate(role, users, (_, _) => users);
    }

    public List<GetUserResponse> GetUsers(string role)
    {
        if (_roleToUserIds.TryGetValue(role, out var users))
        {
            return users;
        }

        throw new InvalidOperationException($"Users for role {role} not found.");
    }
}