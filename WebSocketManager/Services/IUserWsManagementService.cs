namespace WebSocketManager.Services;

public interface IUserWsManagementService
{
    Task<string?> GetHandlerHostAsync(string userId);
    
    Task<Dictionary<string, string?>> GetHandlerHostsAsync(IList<string> userIds); 

    Task SetHandlerAsync(string userId, string wsHandlerUri);
    Task RemoveHandlerAsync(string userId);   
}