using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Common;
using UserService.Responses;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("me")]
    public async Task<ActionResult<GetUserResponse>> Get()
    {
        var getByIdResponse = await _userService.GetByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return getByIdResponse.MatchFirst(
            Ok,
            error => error.ToErrorResponse());
    }

    [HttpPut("me")]
    public async Task<ActionResult> Update(UpdateUserRequest request)
    {   
        var updateResponse = await _userService.UpdateAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, request);

        return updateResponse.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }
}