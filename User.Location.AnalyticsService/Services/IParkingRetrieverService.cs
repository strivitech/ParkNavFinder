using User.Location.AnalyticsService.Domain;

namespace User.Location.AnalyticsService.Services;

public interface IParkingRetrieverService
{
    Task<List<Parking>> GetNextParkingList();
}