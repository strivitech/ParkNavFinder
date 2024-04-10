using Parking.AnalyticsService.Contracts;

namespace Parking.AnalyticsService.Services;

public interface IUserLocationService
{
    Task<IndexUsersResponse> GetUsersAttachedToIndexAsync(string index);
}