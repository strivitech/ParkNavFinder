using DataManager.Api.Contracts;
using DataManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using GetUsersRequest = DataManager.Api.Contracts.GetUsersRequest;

namespace DataManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserManager userManager) : ControllerBase
{
    private readonly IUserManager _userManager = userManager;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetUsersRequest request)
    {   
        var users = await _userManager.GetUsersAsync(request);
        return Ok(users);
    }
}