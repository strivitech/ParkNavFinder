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

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetUsersRequest request)
    {   
        var users = await _userManager.GetUsersAsync(request);
        return Ok(users);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var user = await _userManager.CreateUserAsync(request);
        return Ok(user);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        await _userManager.DeleteUserAsync(new DeleteUserRequest(userId));
        return Ok();
    }
}