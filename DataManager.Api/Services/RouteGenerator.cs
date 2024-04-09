using DataManager.Api.Contracts;
using Polly;
using Polly.Retry;

namespace DataManager.Api.Services;

public class RouteGenerator(IRouteCreator routeCreator, IConfiguration configuration) : IRouteGenerator
{
    private readonly IRouteCreator _routeCreator = routeCreator;
    private readonly double _latitudeMin = configuration.GetValue<double>("Generator:User:LatitudeMin");
    private readonly double _latitudeMax = configuration.GetValue<double>("Generator:User:LatitudeMax");
    private readonly double _longitudeMin = configuration.GetValue<double>("Generator:User:LongitudeMin");
    private readonly double _longitudeMax = configuration.GetValue<double>("Generator:User:LongitudeMax");
    private readonly double _minRouteDistanceKm = configuration.GetValue<double>("Generator:User:MinRouteDistanceKm");
    private readonly int _maxRouteDistanceAttempts =
        configuration.GetValue<int>("Generator:User:MaxRouteDistanceAttempts");

    private const int MaxRouteGenerationAttempts = 5;
    private readonly AsyncRetryPolicy _retryPolicy =
        Policy.Handle<Exception>().RetryAsync(MaxRouteGenerationAttempts);

    public async Task<Route> GenerateRouteAsync() =>
        await _retryPolicy.ExecuteAsync(async () => await GenerateRouteInternalAsync());

    private async Task<Route> GenerateRouteInternalAsync()
    {
        int maxAttempts = _maxRouteDistanceAttempts;
        var bestAttempt = (Start: new Coordinate(0, 0), End: new Coordinate(0, 0), Distance: 0.0);

        while (maxAttempts-- > 0)
        {
            var startCoordinate = GenerateRandomCoordinate();
            var endCoordinate = GenerateRandomCoordinate();
            var currentDistance = GeographicalCalculator.CalculateDistance(startCoordinate, endCoordinate);

            if (currentDistance > bestAttempt.Distance)
            {
                bestAttempt = (startCoordinate, endCoordinate, currentDistance);
            }

            if (currentDistance >= _minRouteDistanceKm)
            {
                break;
            }
        }

        return await _routeCreator.CreateRouteAsync(bestAttempt.Start, bestAttempt.End);
    }


    private Coordinate GenerateRandomCoordinate()
    {
        double latitude = Random.Shared.NextDouble() * (_latitudeMax - _latitudeMin) + _latitudeMin;
        double longitude = Random.Shared.NextDouble() * (_longitudeMax - _longitudeMin) + _longitudeMin;

        return new Coordinate(latitude, longitude);
    }
}