namespace ParkingStateService.Services;

public interface IParkingIndicesRetrieverService
{
    Task<IList<string>> GetNextParkingIndices();
}