namespace Parking.ManagementService.Services;

public interface ICurrentUserService
{
    IUserSessionData SessionData { get; }
}