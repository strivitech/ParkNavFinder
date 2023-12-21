using UserWsHandler.Models;

namespace UserWsHandler.Services;

public interface ILocationService
{
    Task SendLocation(Coordinate coordinate);
}