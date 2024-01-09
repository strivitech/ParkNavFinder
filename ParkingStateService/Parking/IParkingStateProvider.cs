namespace ParkingStateService.Parking;

public interface IParkingStateProvider
{
    Task<List<CurrentParkingState>> GetParkingStatesAsync(string index);  
}