using InvalidOperationException = System.InvalidOperationException;

namespace Parking.StateService.Services;

internal class GeoIndexService(HttpClient httpClient) : IGeoIndexService
{
    private readonly HttpClient _httpClient = httpClient;
    
    public async Task<string> GetGeoIndexAsync(double lat, double lon, int resolution)
    {
        var response = await _httpClient.GetAsync($"api/hexagon?lat={lat}&lon={lon}&resolution={resolution}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<string>();
        return content ?? throw new InvalidOperationException("Response for GeoIndex is null");
    }
}