using DataManager.Api.Contracts;

namespace DataManager.Api.Domain;

public interface IPositionMovable
{
    List<Coordinate> MoveToNextPosition(double maxJumpKilometers);

    bool IsAtDestination { get; }
}