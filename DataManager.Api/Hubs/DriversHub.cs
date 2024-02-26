using DataManager.Api.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace DataManager.Api.Hubs;

public class DriversHub : Hub<IDriversClient>;

public interface IDriversClient
{
    Task ReceiveDriverLocation(string userId, Coordinate coordinate);
}