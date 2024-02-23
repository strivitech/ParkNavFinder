using DataManager.Api.Contracts;
using DataManager.Api.Services;
using Route = DataManager.Api.Services.Route;

namespace DataManager.Api.Domain;

public class Driver : IDriver
{
    private int _lastRoutePositionIndex = 0;

    public Driver(Route route)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentOutOfRangeException.ThrowIfLessThan(route.Coordinates.Count, 2);

        Route = route;
        CurrentPosition = route.Coordinates[0];
    }

    public Route Route { get; }
    public Coordinate CurrentPosition { get; private set; }

    public bool IsAtDestination => _lastRoutePositionIndex == Route.Coordinates.Count - 1;

    public List<Coordinate> MoveToNextPosition(double maxJumpKilometers)
    {
        if (IsAtDestination)
        {
            return [];
        }

        List<Coordinate> path = [];
        double totalDistance = 0;

        for (int i = _lastRoutePositionIndex + 1; i < Route.Coordinates.Count; i++)
        {
            double distanceToNext = GeographicalCalculator.CalculateDistance(CurrentPosition, Route.Coordinates[i]);
            if (totalDistance + distanceToNext > maxJumpKilometers)
            {
                double remainingDistance = maxJumpKilometers - totalDistance;
                Coordinate interpolatedCoordinate = GeographicalCalculator.InterpolateCoordinate(CurrentPosition,
                    Route.Coordinates[i],
                    remainingDistance, distanceToNext);
                path.Add(interpolatedCoordinate);
                CurrentPosition = interpolatedCoordinate;
                break;
            }

            path.Add(Route.Coordinates[i]);
            totalDistance += distanceToNext;
            _lastRoutePositionIndex = i;
            CurrentPosition = Route.Coordinates[i];
        }

        return path;
    }
}