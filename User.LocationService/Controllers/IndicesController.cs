using Auth.Shared;
using Microsoft.AspNetCore.Mvc;
using User.LocationService.Services;

namespace User.LocationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndicesController(IIndicesService indicesService) : ControllerBase
{
    private readonly IIndicesService _indicesService = indicesService;

    [ApiKey(ApiKeyConstants.UserNotificationService, ApiKeyConstants.UserLocationAnalyticsService)]
    [HttpGet("{index}/users")]
    public async Task<IActionResult> GetUsersAttachedToIndex(string index)
    {
        var users = await _indicesService.GetUsersAttachedToIndexAsync(index);
        return users.Count == 0 ? NoContent() : Ok(users);
    }
}