namespace ParkingManagementService.Services;

public interface ICurrentUserService
{
    IUserSessionData SessionData { get; }
}