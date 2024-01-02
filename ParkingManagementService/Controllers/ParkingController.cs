using Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementService.Common;
using ParkingManagementService.Requests;
using ParkingManagementService.Services;

namespace ParkingManagementService.Controllers;

[ApiController]
[Authorize(Roles = Roles.Provider)]
[Route("api/[controller]")]
public class ParkingController : ControllerBase
{
    private readonly IParkingService _parkingService;

    public ParkingController(IParkingService parkingService)
    {
        _parkingService = parkingService;
    }

    public async Task<ActionResult> Add(AddParkingRequest request)
    {
        var response = await _parkingService.AddAsync(request);
        
        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }
    
    public async Task<ActionResult> Update(UpdateParkingRequest request)
    {   
        var response = await _parkingService.UpdateAsync(request);
        
        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }
    
    public async Task<ActionResult> Delete(DeleteParkingRequest request)
    {   
        var response = await _parkingService.DeleteAsync(request);
        
        return response.MatchFirst<ActionResult>(
            _ => Ok(),
            error => error.ToErrorResponse());
    }
}