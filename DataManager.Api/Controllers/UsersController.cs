using DataManager.Api.Common;
using DataManager.Api.Contracts;
using DataManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using GetUsersRequest = DataManager.Api.Contracts.GetUsersRequest;

namespace DataManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserManager userManager, IMemoryCache cache) : ControllerBase
{
    private readonly IUserManager _userManager = userManager;
    private readonly IMemoryCache _cache = cache;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetUsersRequest request)
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
        
        return Ok(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var user = await _userManager.CreateUserAsync(request);
        
        InvalidateRelatedCaches(request.Role);
        
        return Ok(user);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        var userRole = await _userManager.GetUserRoleAsync(new GetUserRoleRequest(userId));
        await _userManager.DeleteUserAsync(new DeleteUserRequest(userId));
        
        InvalidateRelatedCaches(userRole);
        
        return Ok();
    }
    
    private void InvalidateRelatedCaches(string role)
    {
        _cache.Remove($"GetUsers_{role}");  
    }
}