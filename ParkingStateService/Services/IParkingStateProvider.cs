using ParkingStateService.Models;

namespace ParkingStateService.Services;

public interface IParkingStateProvider
{
    Task<List<ActiveParkingState>> GetParkingStatesAsync(string index);  
}