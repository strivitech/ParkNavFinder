using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.ManagementService.Common;
using Parking.ManagementService.Contracts;
using Parking.ManagementService.Services;

namespace Parking.ManagementService.Controllers;

[ApiController]
[Authorize(Roles = Roles.Provider)]
[Route("api/[controller]")]
public class ParkingController(IParkingService parkingService) : ControllerBase
{
    private readonly IParkingService _parkingService = parkingService;

    [AllowAnonymous]
    [HttpGet("{parkingId:guid}")]
    public async Task<ActionResult<GetParkingResponse>> Get(Guid parkingId)
    {
        var response = await _parkingService.GetAsync(new GetParkingRequest(parkingId));

        return response.MatchFirst(
            Ok,
            error => error.ToErrorResponse());
    }
    
    [HttpGet("all-by-provider")]
    public async Task<ActionResult<List<GetParkingResponse>>> GetAllByProvider()
    {
        var response = await _parkingService.GetAllByProviderAsync();
        
        return response.MatchFirst<ActionResult>(
            x => x.Count == 0 ? NoContent() : Ok(x),
            error => error.ToErrorResponse());
    }

    // TODO: Not optimal to return all parkings at once
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<ActionResult<List<GetParkingResponse>>> GetAll()
    {
        var response = await _parkingService.GetAllAsync();

        return response.MatchFirst<ActionResult>(
            x => x.Count == 0 ? NoContent() : Ok(x),
            error => error.ToErrorResponse());
    }

    [HttpPost]
    public async Task<ActionResult> Add(AddParkingRequest request)
    {
        var response = await _parkingService.AddAsync(request);

        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }

    [HttpPut]
    public async Task<ActionResult> Update(UpdateParkingRequest request)
    {
        var response = await _parkingService.UpdateAsync(request);

        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromQuery] DeleteParkingRequest request)
    {
        var response = await _parkingService.DeleteAsync(request);

        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }
}