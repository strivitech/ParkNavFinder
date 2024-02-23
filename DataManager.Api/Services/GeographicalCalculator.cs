using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public static class GeographicalCalculator
{
    private const double EarthRadius = 6371.0; // Radius of the Earth in kilometers


    public static double CalculateDistance(Coordinate from, Coordinate to)
    {
        double latDistance = DegreesToRadians(to.Latitude - from.Latitude);
        double lonDistance = DegreesToRadians(to.Longitude - from.Longitude);
        double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                   Math.Cos(DegreesToRadians(from.Latitude)) * Math.Cos(DegreesToRadians(to.Latitude)) *
                   Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadius * c; // Distance in kilometers
    }

    public static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);

    public static Coordinate InterpolateCoordinate(Coordinate from, Coordinate to, double distanceToInterpolate,
        double totalDistance)
    {
        // Convert lat/long to Cartesian coordinates for a spherical Earth
        (double x1, double y1, double z1) = LatLongToCartesian(from.Latitude, from.Longitude);
        (double x2, double y2, double z2) = LatLongToCartesian(to.Latitude, to.Longitude);

        // Perform linear interpolation in Cartesian coordinates
        double fraction = distanceToInterpolate / totalDistance;
        double x = x1 + fraction * (x2 - x1);
        double y = y1 + fraction * (y2 - y1);
        double z = z1 + fraction * (z2 - z1);

        // Convert back to lat/long
        return CartesianToLatLong(x, y, z);
    }

    public static (double, double, double) LatLongToCartesian(double latitude, double longitude)
    {
        double latRad = DegreesToRadians(latitude);
        double lonRad = DegreesToRadians(longitude);
        double x = EarthRadius * Math.Cos(latRad) * Math.Cos(lonRad);
        double y = EarthRadius * Math.Cos(latRad) * Math.Sin(lonRad);
        double z = EarthRadius * Math.Sin(latRad);

        return (x, y, z);
    }

    public static Coordinate CartesianToLatLong(double x, double y, double z)
    {
        double latRad = Math.Asin(z / EarthRadius);
        double lonRad = Math.Atan2(y, x);
        double latitude = RadiansToDegrees(latRad);
        double longitude = RadiansToDegrees(lonRad);

        return new Coordinate(latitude, longitude);
    }

    public static double RadiansToDegrees(double radians) => radians * (180.0 / Math.PI);
}