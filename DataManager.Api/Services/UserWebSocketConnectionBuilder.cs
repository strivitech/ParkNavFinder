using Microsoft.AspNetCore.SignalR.Client;

namespace DataManager.Api.Services;

public class UserWebSocketConnectionBuilder(ITokenStorage tokenStorage) : IUserWebSocketConnectionBuilder
{
    private readonly ITokenStorage _tokenStorage = tokenStorage;

    public HubConnection BuildConnection(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        // TODO: change when Aspire fixes matching with SignalR
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5002/api/usershub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_tokenStorage.GetToken(userId))!;
            })
            .WithAutomaticReconnect()
            .Build();
        
        return connection;
    }
}