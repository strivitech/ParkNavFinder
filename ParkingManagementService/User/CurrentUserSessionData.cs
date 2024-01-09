using System.Security.Claims;

namespace ParkingManagementService.User;

internal class CurrentUserSessionData(IHttpContextAccessor httpContextAccessor) : IUserSessionData
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? throw new InvalidOperationException("User id not found because user is not authenticated");
}