namespace LocationService.UserLocation;

public interface IUserLocationService
{
    Task PostNewLocation(PostUserLocationRequest postUserLocationRequest);
}