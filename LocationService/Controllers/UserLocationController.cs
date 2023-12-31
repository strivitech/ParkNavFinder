﻿using Auth.Shared;
using LocationService.Requests;
using LocationService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserLocationController(IUserLocationService userLocationService) : ControllerBase
{
    private readonly IUserLocationService _userLocationService = userLocationService;
    
    [ApiKey(ApiKeyConstants.UserWsHandler)]
    [HttpPost]
    public async Task<IActionResult> PostNewLocation([FromBody] PostUserLocationRequest postUserLocationRequest)
    {
        await _userLocationService.PostNewLocation(postUserLocationRequest);
        
        return Ok();
    }
}