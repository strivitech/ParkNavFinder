using Parking.StateService.Domain;

namespace Parking.StateService.Services;

public interface IParkingStateProvider
{
    Task<List<ParkingState>> GetParkingStatesAsync(string index);  
}