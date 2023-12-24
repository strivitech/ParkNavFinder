using Auth.Shared;
using H3;
using H3.Model;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace MapService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HexagonController : ControllerBase
{
    /// <summary>
    /// Get the hexagon that contains the provided coordinates. 
    /// </summary>
    /// <param name="lat">Latitude in degrees</param>
    /// <param name="lon">Longitude in degrees</param>
    /// <param name="resolution">The resolution of the hexagon</param>
    /// <returns></returns> 
    [ApiKey(ApiKeyConstants.UserWsHandler)]
    [HttpGet]
    public IActionResult GetH3Index(double lat, double lon, int resolution)
    {   
        var latLng = LatLng.FromCoordinate(new Coordinate(lon, lat));
        var h3Index = H3Index.FromLatLng(latLng, resolution);

        return Ok(h3Index);
    }
}