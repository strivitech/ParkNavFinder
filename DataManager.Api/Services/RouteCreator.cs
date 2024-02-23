using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public class RouteCreator(IMapService mapService) : IRouteCreator
{
    private readonly IMapService _mapService = mapService;

    public async Task<Route> CreateRouteAsync(Coordinate start, Coordinate end) =>
        await _mapService.GetRouteAsync(start, end);
}