using DataManager.Api.Contracts;

namespace DataManager.Api.Domain;

public interface IPositionMovable
{
    Coordinate MoveToNextPosition(double maxJumpKilometers);

    bool IsAtDestination { get; }
}