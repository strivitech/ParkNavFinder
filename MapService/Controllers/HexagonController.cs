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
    [ApiKey(ApiKeyConstants.UserLocationService, ApiKeyConstants.ParkingStateService)]
    [HttpGet]
    public ActionResult<H3Index> GetH3Index(double lat, double lon, int resolution) 
    {
        var validationMessage = ValidateCoordinatesAndResolution(lat, lon, resolution);
        if (validationMessage is not null)
        {
            return BadRequest(validationMessage);
        }

        var latLng = LatLng.FromCoordinate(new Coordinate(lon, lat));
        var h3Index = H3Index.FromLatLng(latLng, resolution);

        return Ok(h3Index.ToString());
    }

    private static string? ValidateCoordinatesAndResolution(double lat, double lon, int resolution)
    {
        if (lat is < -90 or > 90) 
        {
            return "Latitude must be between -90 and 90.";
        }

        if (lon is < -180 or > 180) 
        {
            return "Longitude must be between -180 and 180.";
        }
        
        if (resolution is < 0 or > 15) 
        {
            return "Resolution must be within an acceptable range.";
        }

        return null;
    }
}