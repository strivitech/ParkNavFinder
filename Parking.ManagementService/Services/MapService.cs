namespace Parking.ManagementService.Services;


public class MapService(HttpClient httpClient) : IMapService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> GetIndexAsync(double lat, double lon, int resolution)
    {
        var response = await _httpClient.GetAsync($"api/hexagon?lat={lat}&lon={lon}&resolution={resolution}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content ?? throw new InvalidOperationException("Response content is null.");
    }
}