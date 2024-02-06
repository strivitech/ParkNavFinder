using User.NotificationService.Contracts;

namespace User.NotificationService.Services;

public interface IUserLocationService
{
    Task<IndexUsersResponse> GetUsersAttachedToIndex(string index);
}