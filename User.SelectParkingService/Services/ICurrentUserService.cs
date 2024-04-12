namespace User.SelectParkingService.Services;

public interface ICurrentUserService
{
    IUserSessionData SessionData { get; }
}