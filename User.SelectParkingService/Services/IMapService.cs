using User.SelectParkingService.Contracts;
using Route = User.SelectParkingService.Contracts.Route;

namespace User.SelectParkingService.Services;

public interface IMapService
{
    Task<Route> GetRouteAsync(Coordinate start, Coordinate end);
}