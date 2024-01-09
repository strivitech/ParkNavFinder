using Auth.Shared;
using H3;
using H3.Model;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace MapService.GeoIndex;

[Route("api/[controller]")]
[ApiController]
public class HexagonController : ControllerBase
{
    [ApiKey(ApiKeyConstants.UserActiveGeoIndexService)]
    [HttpGet]
    public ActionResult<H3Index> GetH3Index(double lat, double lon, int resolution) 
    {   
        var latLng = LatLng.FromCoordinate(new Coordinate(lon, lat));
        var h3Index = H3Index.FromLatLng(latLng, resolution);

        return Ok(h3Index);
    }
}