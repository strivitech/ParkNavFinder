using System.Runtime.Serialization;
using User.NotificationService.Contracts;

namespace User.NotificationService.Services;

public class UserLocationService(HttpClient httpClient) : IUserLocationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IndexUsersResponse> GetUsersAttachedToIndex(string index)
    {
        var response = await _httpClient.GetAsync($"api/indices/{index}/users");
        response.EnsureSuccessStatusCode();

        var userIds = await response.Content.ReadFromJsonAsync<List<string>>() ??
                      throw new SerializationException("Failed to deserialize response");
        return new IndexUsersResponse(userIds);
    }
}