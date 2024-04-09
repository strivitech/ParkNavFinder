using User.Location.AnalyticsService.Contracts;

namespace User.Location.AnalyticsService.Services;

public interface IParkingStatesCalculator
{
    Task<List<ParkingAnalyticsData>> CalculateNextBatchAsync();
}