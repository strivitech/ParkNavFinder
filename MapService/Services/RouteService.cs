using MapService.Contracts;
using Route = MapService.Contracts.Route;

namespace MapService.Services;

public class RouteService(HttpClient httpClient) : IRouteService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Route> GetRouteAsync(Coordinate start, Coordinate end)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"ors/v2/directions/driving-car?start={start.Longitude},{start.Latitude}&end={end.Longitude},{end.Latitude}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OpenRouteResponse>() is { } openRouteResponse
            ? new Route(openRouteResponse.Features.First().Properties.Summary.Distance,
                openRouteResponse.Features.First().Geometry.Coordinates.Select(c => new Coordinate(c[1], c[0])).ToList())
            : throw new InvalidOperationException("No route returned");
    }
}