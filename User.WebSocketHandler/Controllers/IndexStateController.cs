using Auth.Shared;
using Microsoft.AspNetCore.Mvc;
using User.WebSocketHandler.Contracts;
using User.WebSocketHandler.Services;

namespace User.WebSocketHandler.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class IndexStateController(IIndexStateNotificationService indexStateNotificationService)
    : ControllerBase
{
    private readonly IIndexStateNotificationService
        _indexStateNotificationService = indexStateNotificationService;

    [HttpPost]
    [ApiKey(ApiKeyConstants.UserNotificationService)]
    public async Task<IActionResult> Notify(IndexStateNotification indexStateNotification)
    {
        await _indexStateNotificationService.NotifyUsersAsync(indexStateNotification.ReceiverIds,
            indexStateNotification.State);
        return Ok();
    }
}