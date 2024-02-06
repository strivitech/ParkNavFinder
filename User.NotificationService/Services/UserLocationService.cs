using System.Text.Json;
using User.NotificationService.Contracts;

namespace User.NotificationService.Services;

internal class UserLocationService(HttpClient httpClient) : IUserLocationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IndexUsersResponse> GetUsersAttachedToIndex(string index)
    {
        var response = await _httpClient.GetAsync($"api/indices/{index}/users");

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Error while getting users attached to index {index}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var userIds = JsonSerializer.Deserialize<List<string>>(content) ?? [];
        return new IndexUsersResponse(userIds);
    }
}