using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementService.Common;
using ParkingManagementService.Requests;
using ParkingManagementService.Responses;
using ParkingManagementService.Services;

namespace ParkingManagementService.Controllers;

[ApiController]
[Authorize(Roles = Roles.Provider)]
[Route("api/[controller]")]
public class ParkingController(IParkingService parkingService) : ControllerBase
{
    private readonly IParkingService _parkingService = parkingService;

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<GetParkingResponse>> Get([FromQuery] GetParkingRequest request)
    {
        var response = await _parkingService.GetAsync(request);

        return response.MatchFirst(
            Ok,
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