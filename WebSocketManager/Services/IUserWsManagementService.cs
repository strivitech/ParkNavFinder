namespace WebSocketManager.Services;

public interface IUserWsManagementService
{
    Task<string?> GetWebSocketHandlerHostByUserIdAsync(string userId);

    Task SetWebSocketHandlerHostForUserIdAsync(string userId, string wsHandlerUri);
    Task RemoveWebSocketHandlerHostForUserIdAsync(string userId);   
}