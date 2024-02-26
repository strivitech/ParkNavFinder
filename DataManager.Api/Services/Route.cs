using System.Collections.Immutable;
using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public record Route(double Distance, ImmutableList<Coordinate> Coordinates);