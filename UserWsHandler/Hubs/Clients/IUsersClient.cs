using UserWsHandler.Models;

namespace UserWsHandler.Hubs.Clients;

public interface IUsersClient
{
    Task ParkingInfoUpdated(List<Parking> updatedParkingList);
}