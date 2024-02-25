using MapService.Contracts;
using Route = MapService.Contracts.Route;

namespace MapService.Services;

public interface IRouteService
{
    Task<Route> GetRouteAsync(Coordinate start, Coordinate end);
}