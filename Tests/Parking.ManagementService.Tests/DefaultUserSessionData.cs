using Parking.ManagementService.Services;

namespace Parking.ManagementService.Tests;

public class DefaultUserSessionData : IUserSessionData
{
    public string UserId { get; set; } = "UserId";
}