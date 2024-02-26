using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IMapService
{
    Task<Route> GetRouteAsync(Coordinate start, Coordinate end);
}