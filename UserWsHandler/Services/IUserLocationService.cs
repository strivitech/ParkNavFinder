using UserWsHandler.Models;

namespace UserWsHandler.Services;

public interface IUserLocationService
{   
    Task SendLocation(string userId, Coordinate coordinate);
}