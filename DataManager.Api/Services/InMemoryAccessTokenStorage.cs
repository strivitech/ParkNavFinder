using System.Collections.Concurrent;

namespace DataManager.Api.Services;

public sealed class InMemoryAccessTokenStorage : ITokenStorage
{
    private readonly ConcurrentDictionary<string, string> _userIdToToken = new();

    public void StoreToken(string userId, string token)
    {
        ArgumentException.ThrowIfNullOrEmpty(token);
        
        _userIdToToken.AddOrUpdate(userId, token, (_, _) => token);
    }

    public string GetToken(string userId)
    {
        if (_userIdToToken.TryGetValue(userId, out var token))
        {
            return token;
        }

        throw new InvalidOperationException($"Token for user ID {userId} not found.");
    }
}