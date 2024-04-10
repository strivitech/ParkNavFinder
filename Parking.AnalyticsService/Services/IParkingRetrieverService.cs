namespace Parking.AnalyticsService.Services;

public interface IParkingRetrieverService
{
    Task<List<Domain.Parking>> GetNextParkingList();
}