using User.Location.AnalyticsService.Contracts;

namespace User.Location.AnalyticsService.Services;

public interface IUserLocationService
{
    Task<IndexUsersResponse> GetUsersAttachedToIndexAsync(string index);
}