using LocationService.Requests;

namespace LocationService.Services;

public interface IUserLocationService
{
    Task PostNewLocation(PostUserLocationRequest postUserLocationRequest);
}