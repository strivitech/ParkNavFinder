using DataManager.Api.Contracts;

namespace DataManager.Api.Domain;

public interface ICurrentPosition
{
    Coordinate CurrentPosition { get; }
}