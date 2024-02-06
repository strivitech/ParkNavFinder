using User.NotificationService.Contracts;

namespace User.NotificationService.Services;

public interface IWebsocketManager
{
    Task<List<UserHandlerDescription>> GetHandlersAsync(IList<string> userIds);
}