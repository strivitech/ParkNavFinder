using System.Net;
using System.Runtime.Serialization;
using Parking.AnalyticsService.Contracts;

namespace Parking.AnalyticsService.Services;

public class UserLocationService(HttpClient httpClient) : IUserLocationService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IndexUsersResponse> GetUsersAttachedToIndexAsync(string index)
    {
        var response = await _httpClient.GetAsync($"api/indices/{index}/users");
        response.EnsureSuccessStatusCode();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var userIds = await response.Content.ReadFromJsonAsync<List<string>>() ??
                          throw new SerializationException("Failed to deserialize response");
            return new IndexUsersResponse(userIds);
        }

        return new IndexUsersResponse([]);
    }
}