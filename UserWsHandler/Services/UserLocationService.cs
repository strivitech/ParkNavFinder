using UserWsHandler.Models;

namespace UserWsHandler.Services;

internal class UserLocationService(HttpClient httpClient, ILogger<UserLocationService> logger) : IUserLocationService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<UserLocationService> _logger = logger;
        
    public async Task SendLocation(string userId, Coordinate coordinate)
    {   
        _logger.LogDebug("Sending location for user {UserId}", userId);
        
        const string requestUri = "api/UserLocation";
        var message = new UserLocationMessage(userId, coordinate.Latitude, coordinate.Longitude, DateTime.UtcNow);
        var content = JsonContent.Create(message);
        _ = await _httpClient.PostAsync(requestUri, content);
        
        _logger.LogDebug("Location sent for user {UserId}", userId);
    }
}

