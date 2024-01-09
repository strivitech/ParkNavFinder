namespace ParkingManagementService.User;

public interface ICurrentUserService
{
    IUserSessionData SessionData { get; }
}