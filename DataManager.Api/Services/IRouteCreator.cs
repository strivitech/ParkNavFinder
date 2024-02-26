using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IRouteCreator
{
    Task<Route> CreateRouteAsync(Coordinate start, Coordinate end);
}