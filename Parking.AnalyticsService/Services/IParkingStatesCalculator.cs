using Parking.AnalyticsService.Contracts;

namespace Parking.AnalyticsService.Services;

public interface IParkingStatesCalculator
{
    Task<List<ParkingAnalyticsData>> CalculateNextBatchAsync();
}