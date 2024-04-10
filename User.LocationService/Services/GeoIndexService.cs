using InvalidOperationException = System.InvalidOperationException;

namespace User.LocationService.Services;

public class GeoIndexService(HttpClient httpClient) : IGeoIndexService
{
    private readonly HttpClient _httpClient = httpClient;
    
    public async Task<string> GetGeoIndexAsync(double lat, double lon, int resolution)
    {
        var response = await _httpClient.GetAsync($"api/hexagon?lat={lat}&lon={lon}&resolution={resolution}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content ?? throw new InvalidOperationException("Response for GeoIndex is null");
    }

    public async Task<List<string>> GetRingIndicesAsync(double lat, double lon, int resolution, int k)
    {
        var response = await _httpClient.GetAsync($"api/hexagon/GetRingIndices?lat={lat}&lon={lon}&resolution={resolution}&k={k}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<List<string>>();
        return content ?? throw new InvalidOperationException("Response for RingIndices is null");
    }
}