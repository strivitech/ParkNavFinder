using Auth.Shared;
using MapService.Contracts;
using MapService.Services;
using Microsoft.AspNetCore.Mvc;
using Route = MapService.Contracts.Route;

namespace MapService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RouteController(IRouteService routeService) : ControllerBase
{
    private readonly IRouteService _routeService = routeService;

    [HttpGet]
    [ApiKey(ApiKeyConstants.DataManagerApi)]
    public async Task<ActionResult<Route>> GetRoute(double startLat, double startLong, double endLat, double endLong)
    {
        var route = await _routeService.GetRouteAsync(new Coordinate(startLat, startLong), new Coordinate(endLat, endLong));
        return Ok(route);
    }
}