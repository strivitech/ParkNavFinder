using Microsoft.AspNetCore.SignalR;
using UserWsHandler.Hubs.Clients;

namespace UserWsHandler.Hubs;

public class UsersHub : Hub<IUsersClient>
{
}
