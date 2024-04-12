using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.SelectParkingService.Common;
using User.SelectParkingService.Contracts;
using User.SelectParkingService.Services;
using Route = User.SelectParkingService.Contracts.Route;

namespace User.SelectParkingService.Controllers;

[ApiController]
[Authorize(Roles = Roles.User)]
[Route("api/[controller]")]
public class SelectParkingController(ISelectParkingService selectParkingService) : ControllerBase
{
    private readonly ISelectParkingService _selectParkingService = selectParkingService;

    [HttpPost]
    public async Task<ActionResult<Route>> SelectRoute(SelectParkingRequest request)
    {
        var response = await _selectParkingService.SelectParkingAsync(request);

        return response.MatchFirst<ActionResult>(
            Ok,
            error => error.ToErrorResponse());
    }
}