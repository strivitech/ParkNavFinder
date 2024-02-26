using DataManager.Api.Contracts;
using DataManager.Api.Services;
using Route = DataManager.Api.Services.Route;

namespace DataManager.Api.Domain;

public class Driver : IDriver
{
    private int _currentRouteIndex = 0;

    public Driver(Route route)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentOutOfRangeException.ThrowIfLessThan(route.Coordinates.Count, 2);

        Route = route;
        CurrentPosition = route.Coordinates[0];
    }

    public Route Route { get; }
    public Coordinate CurrentPosition { get; private set; }

    public bool IsAtDestination => _currentRouteIndex >= Route.Coordinates.Count - 1;

    public Coordinate MoveToNextPosition(double maxJumpKilometers)
    {
        if (IsAtDestination)
        {
            return CurrentPosition;
        }

        double remainingDistance = maxJumpKilometers;

        while (_currentRouteIndex < Route.Coordinates.Count - 1 && remainingDistance > 0)
        {
            Coordinate nextPoint = Route.Coordinates[_currentRouteIndex + 1];
            double distanceToNext = GeographicalCalculator.CalculateDistance(CurrentPosition, nextPoint);

            if (distanceToNext <= remainingDistance)
            {
                // Move to the next point and update remaining distance
                CurrentPosition = nextPoint;
                remainingDistance -= distanceToNext;
                _currentRouteIndex++;
            }
            else
            {
                // Move partially towards the next point
                CurrentPosition = GeographicalCalculator.InterpolateCoordinate(
                    CurrentPosition, nextPoint, remainingDistance, distanceToNext);
                remainingDistance = 0; // No more distance can be covered
            }
        }

        return CurrentPosition;
    }
}