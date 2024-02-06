namespace User.WebSocketHandler.Services;

internal class WsManagerService(HttpClient httpClient, ILogger<WsManagerService> logger) : IWsManagerService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<WsManagerService> _logger = logger;
    
    public async Task SendUserConnectedMessage(string userId)
    {
        _logger.LogInformation("Sending user connected message for user {UserId}", userId);
        
        const string requestUri = "api/UserWsManagement/SetHandler";
        var content = JsonContent.Create(userId);
        var response = await _httpClient.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
        
        _logger.LogInformation("Successfully connected WebSocket handler for user {UserId}", userId);
    }
    
    public async Task SendUserDisconnectedMessage(string userId)    
    {
        _logger.LogInformation("Sending user disconnected message for user {UserId}", userId);
        
        string requestUri = $"api/UserWsManagement/RemoveWebSocketHandlerHostForUserId/{userId}";
        var response = await _httpClient.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
        
        _logger.LogInformation("Successfully disconnected WebSocket handler for user {UserId}", userId);
    }
}