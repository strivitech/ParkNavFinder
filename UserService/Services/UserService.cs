using System.Text.Json;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using UserService.Common;
using UserService.Models;
using UserService.Responses;

namespace UserService.Services;

internal class UserService(UserManager<User> userManager, IDistributedCache distributedCache) : IUserService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<ErrorOr<GetUserResponse>> GetByIdAsync(string userId)
    {
        var cacheKey = CacheKeys.UserKey(userId);
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<GetUserResponse>(cachedData)!;
        }
        
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Errors.User.NotFound();
        }

        var response = new GetUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
        
        var serializedData = JsonSerializer.Serialize(response);
        await _distributedCache.SetStringAsync(cacheKey, serializedData);

        return response;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(string userId, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Errors.User.NotFound();
        }

        user.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Errors.User.UpdateFailed();
        }

        var cacheKey = CacheKeys.UserKey(userId);
        await _distributedCache.RemoveAsync(cacheKey);

        return new Updated();
    }
}