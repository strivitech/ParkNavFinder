using DataManager.Api.Common;
using DataManager.Api.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace DataManager.Api.Services;

public class CachingUserManager(IUserManager userManager, IMemoryCache cache) : IUserManager
{
    private readonly IUserManager _userManager = userManager;
    private readonly IMemoryCache _cache = cache;

    public async Task<List<GetUserResponse>> GetUsersAsync(GetUsersRequest request)
    {
        var cacheKey = $"GetUsers_{request.GetUniqueIdentifier()}";
        if (!_cache.TryGetValue(cacheKey, out List<GetUserResponse>? users))
        {
            users = await _userManager.GetUsersAsync(request);
            _cache.Set(cacheKey, users, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
        }
        
        return users!;
    }

    public async Task<string> GetUserRoleAsync(GetUserRoleRequest request) =>
        await _userManager.GetUserRoleAsync(request);

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var user = await _userManager.CreateUserAsync(request);
        InvalidateRelatedCaches(request.Role);
        return user;
    }

    public async Task DeleteUserAsync(DeleteUserRequest request)
    {
        var userRole = await _userManager.GetUserRoleAsync(new GetUserRoleRequest(request.UserId));
        await _userManager.DeleteUserAsync(request);
        InvalidateRelatedCaches(userRole);
    }

    private void InvalidateRelatedCaches(string role) => _cache.Remove($"GetUsers_{role}");
}