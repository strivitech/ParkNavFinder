using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace ParkingManagementService.Common;

public static class ControllerExtensions
{
    public static ObjectResult ToErrorResponse(this Error error)
    {
        var errorObject = new
        {
            error.Code,
            error.Description
        };
        
        return error.Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(errorObject),
            ErrorType.Failure => new BadRequestObjectResult(errorObject),
            ErrorType.Unexpected => new ObjectResult(errorObject) { StatusCode = StatusCodes.Status500InternalServerError },
            ErrorType.Validation => new BadRequestObjectResult(errorObject),
            ErrorType.Conflict => new ConflictObjectResult(errorObject),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(errorObject),
            _ => new ObjectResult(errorObject) { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }
}