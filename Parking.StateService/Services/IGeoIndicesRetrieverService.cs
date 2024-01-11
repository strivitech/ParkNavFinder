namespace Parking.StateService.Services;

public interface IGeoIndicesRetrieverService
{
    Task<IList<string>> GetNextParkingIndices();
}