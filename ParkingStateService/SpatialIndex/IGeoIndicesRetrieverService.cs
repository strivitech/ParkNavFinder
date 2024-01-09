namespace ParkingStateService.SpatialIndex;

public interface IGeoIndicesRetrieverService
{
    Task<IList<string>> GetNextParkingIndices();
}