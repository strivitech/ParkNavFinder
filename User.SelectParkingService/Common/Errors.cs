using ErrorOr;

namespace User.SelectParkingService.Common;

public static class Errors
{
    public static class Parking
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("Parking.NotFound", $"Parking with id {id.ToString()} not found.");

        public static Error SelectParkingFailed() =>
            Error.Unexpected("Parking.SelectFailed", "Failed to select parking.");
    }

    public static class Map
    {
        public static Error RouteNotFound() =>
            Error.NotFound("Map.RouteNotFound", "Route not found.");
    }
}