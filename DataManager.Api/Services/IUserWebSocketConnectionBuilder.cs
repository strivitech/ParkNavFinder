using Microsoft.AspNetCore.SignalR.Client;

namespace DataManager.Api.Services;

public interface IUserWebSocketConnectionBuilder
{
    HubConnection BuildConnection(string userId);
}