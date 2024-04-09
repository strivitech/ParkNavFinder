using User.Location.AnalyticsService.Contracts;

namespace User.Location.AnalyticsService.Services;

public class ParkingStatesCalculator(
    IParkingRetrieverService parkingRetrieverService,
    IUserLocationService userLocationService,
    ILogger<ParkingStatesCalculator> logger) : IParkingStatesCalculator
{
    private readonly IParkingRetrieverService _parkingRetrieverService = parkingRetrieverService;
    private readonly IUserLocationService _userLocationService = userLocationService;
    private readonly ILogger<ParkingStatesCalculator> _logger = logger;

    public async Task<List<ParkingAnalyticsData>> CalculateNextBatchAsync()
    {
        _logger.LogDebug("Calculating parking states");

        var parkingData = await _parkingRetrieverService.GetNextParkingList();

        var uniqueGeoIndices = parkingData.Select(p => p.GeoIndex).Distinct().ToList();

        var tasks = uniqueGeoIndices.Select(async geoIndex =>
        {
            var attachedUsers = await _userLocationService.GetUsersAttachedToIndexAsync(geoIndex);
            return new { GeoIndex = geoIndex, AttachedUsers = attachedUsers.UserIds };
        }).ToList();

        var results = await Task.WhenAll(tasks);

        var geoIndexAttachedUsers =
            results.ToDictionary(result => result.GeoIndex, result => result.AttachedUsers);

        return parkingData.Select(parking => new ParkingAnalyticsData(ParkingId: parking.ParkingId,
            TotalObservers: geoIndexAttachedUsers[parking.GeoIndex].Count,
            Probability: CalculateProbability(parking.TotalSpaces, geoIndexAttachedUsers[parking.GeoIndex].Count),
            CalculatedAtUtc: DateTime.UtcNow)).ToList();
    }
    
    private static double CalculateProbability(int totalSpaces, int totalObservers)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(totalSpaces, 0, nameof(totalSpaces));
        ArgumentOutOfRangeException.ThrowIfLessThan(totalObservers, 0, nameof(totalObservers));
        
        if (totalObservers == 0)
        {
            return 0;
        }

        if (totalSpaces == 0)
        {
            return 1.0;
        }
        
        var probability = (double) totalSpaces / totalObservers;
        
        return probability > 1.0 ? 1.0 : probability;
    }
}