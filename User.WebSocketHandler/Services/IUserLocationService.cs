using User.WebSocketHandler.Contracts;

namespace User.WebSocketHandler.Services;

public interface IUserLocationService
{
    Task PostLocationAsync(PostUserLocationRequest postUserLocationRequest);
}