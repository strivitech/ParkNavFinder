using Auth.Shared;
using Microsoft.AspNetCore.Mvc;

namespace WebSocketManager.UserWs;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserWsManagementController(IUserWsManagementService userWsService) : ControllerBase
{
    private readonly IUserWsManagementService _userWsManagementService = userWsService;
    
    // TODO: Set [ApiKey(ApiKeyConstants.SomeOtherApiKey)]
    [HttpGet]
    public async Task<ActionResult<string>> GetWebSocketHandlerHostByUserId(string userId)
    {
        var wsHandlerUri = await _userWsManagementService.GetWebSocketHandlerHostByUserIdAsync(userId);

        return wsHandlerUri is null ? NotFound() : Ok(wsHandlerUri);
    }
    
    [ApiKey(ApiKeyConstants.UserWsHandler)]
    [HttpPost]
    public async Task<IActionResult> SetWebSocketHandlerHostForUserId([FromBody] string userId)
    {
        var wsHandlerHost = $"{Request.Scheme}://{Request.Host}/";   
        
        await _userWsManagementService.SetWebSocketHandlerHostForUserIdAsync(userId, wsHandlerHost);
        return Ok();
    }
    
    [ApiKey(ApiKeyConstants.UserWsHandler)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> RemoveWebSocketHandlerHostForUserId(string userId)
    {
        await _userWsManagementService.RemoveWebSocketHandlerHostForUserIdAsync(userId);
        return Ok();
    }
}