using System.Text.Json;
using User.NotificationService.Contracts;

namespace User.NotificationService.Services;

internal class WebsocketManager(HttpClient httpClient) : IWebsocketManager
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<UserHandlerDescription>> GetHandlersAsync(IList<string> userIds)
    {
        const string requestUri = "api/UserWsManagement/GetHandlerHosts";
        var body = JsonContent.Create(userIds);
        var response = await _httpClient.PostAsync(requestUri, body);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var idsHandlers = JsonSerializer.Deserialize<Dictionary<string, string?>>(content) ?? [];
        return idsHandlers.Select(x => 
                new UserHandlerDescription { UserId = x.Key, Handler = x.Value })
            .ToList();
    }
}