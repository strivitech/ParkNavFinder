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

    public Coordinate MoveToNextPosition(double maxJumpKilometers)
    {
        if (IsAtDestination)
        {
            return CurrentPosition;
        }

        double totalDistance = 0;
        int currentIndex = _lastRoutePositionIndex;

        while (currentIndex + 1 < Route.Coordinates.Count)
        {
            double distanceToNext = CalculateDistanceToNext(currentIndex);

            if (totalDistance + distanceToNext > maxJumpKilometers)
            {
                double remainingDistance = maxJumpKilometers - totalDistance;
                return UpdatePosition(currentIndex, GeographicalCalculator.InterpolateCoordinate(
                    Route.Coordinates[currentIndex], Route.Coordinates[currentIndex + 1], remainingDistance,
                    distanceToNext));
            }

            totalDistance += distanceToNext;
            currentIndex++;
        }

        return UpdatePosition(currentIndex, Route.Coordinates[currentIndex]);
    }

    private double CalculateDistanceToNext(int currentIndex) =>
        GeographicalCalculator.CalculateDistance(Route.Coordinates[currentIndex], Route.Coordinates[currentIndex + 1]);

    private Coordinate UpdatePosition(int newIndex, Coordinate newPosition)
    {
        _lastRoutePositionIndex = newIndex;
        CurrentPosition = newPosition;
        return newPosition;
    }
}