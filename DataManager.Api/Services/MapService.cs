using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public class MapService(HttpClient httpClient) : IMapService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Route> GetRouteAsync(Coordinate start, Coordinate end)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"api/route?startLat={start.Latitude}&startLong={start.Longitude}&endLat={end.Latitude}&endLong={end.Longitude}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Route>() ??
               throw new InvalidOperationException("No route returned");
    }
}