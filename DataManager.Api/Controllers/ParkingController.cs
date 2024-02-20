using Auth.Shared;
using DataManager.Api.Common;
using DataManager.Api.Contracts;
using DataManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ParkingController(IUserManager userManager, IParkingManager parkingManager, ITokenStorage tokenStorage)
    : ControllerBase
{
    private readonly IUserManager _userManager = userManager;
    private readonly IParkingManager _parkingManager = parkingManager;
    private readonly ITokenStorage _tokenStorage = tokenStorage;

    [HttpGet]
    public async Task<ActionResult<List<GetParkingResponse>>> GetAll()
    {
        var providers = await _userManager.GetUsersAsync(new GetUsersRequest(Roles.Provider));
        var generatedProviders = providers.Where(x => x.Email.Contains(Constants.GeneratedEmailSharedKey)).ToList();

        if (generatedProviders.Count == 0)
        {
            return NoContent();
        }

        var listOfAllParking = new List<GetParkingResponse>();
        foreach (var token in generatedProviders.Select(generatedProvider =>
                     _tokenStorage.GetToken(generatedProvider.UserId)))
        {
            var parking = await _parkingManager.GetAllByProviderAsync(token);
            listOfAllParking.AddRange(parking);
        }
        
        return Ok(listOfAllParking);
    }
}