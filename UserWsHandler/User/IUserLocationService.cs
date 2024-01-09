namespace UserWsHandler.User;

public interface IUserLocationService
{   
    Task SendLocation(string userId, Coordinate coordinate);
}