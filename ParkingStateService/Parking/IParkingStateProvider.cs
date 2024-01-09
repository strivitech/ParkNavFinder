namespace ParkingStateService.Parking;

public interface IParkingStateProvider
{
    Task<List<ParkingStateModel>> GetParkingStatesAsync(string index);  
}