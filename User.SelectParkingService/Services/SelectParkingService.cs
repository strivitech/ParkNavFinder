using ErrorOr;
using User.SelectParkingService.Common;
using User.SelectParkingService.Contracts;
using User.SelectParkingService.Database;
using User.SelectParkingService.Domain;
using Route = User.SelectParkingService.Contracts.Route;

namespace User.SelectParkingService.Services;

public class SelectParkingService(
    ICurrentUserService currentUserService,
    IMapService mapService,
    IParkingManagementService parkingManagementService,
    ILogger<SelectParkingService> logger,
    ParkingSelectionDbContext dbContext) : ISelectParkingService
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IParkingManagementService _parkingManagementService = parkingManagementService;
    private readonly IMapService _mapService = mapService;
    private readonly ILogger<SelectParkingService> _logger = logger;
    private readonly ParkingSelectionDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Route>> SelectParkingAsync(SelectParkingRequest request)
    {
        string userId = _currentUserService.SessionData.UserId;
        
        var parkingResponse = await _parkingManagementService.GetParkingCoordsAsync(request.ParkingId);
        
        if (parkingResponse.IsError)
        {
            return parkingResponse.Errors;
        }
        
        var routeOrError = await GetRouteAsync(request, parkingResponse);

        return await routeOrError.MatchAsync<ErrorOr<Route>>(
            async route => {
                await SaveUserParkingSelection(request, userId);
                return route;
            },
            errors => Task.FromResult<ErrorOr<Route>>(errors)
        );
    }

    private async Task SaveUserParkingSelection(SelectParkingRequest request, string userId)
    {
        var userParkingSelection = new UserParkingSelection
        {
            UserId = userId,
            ParkingId = request.ParkingId,
            DateTimeUtc = DateTime.UtcNow
        };
        
        _dbContext.UserParkingSelections.Add(userParkingSelection);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<ErrorOr<Route>> GetRouteAsync(SelectParkingRequest request, ErrorOr<Coordinate> parkingResponse)
    {
        try
        {
            var route = await _mapService.GetRouteAsync(request.UserPosition, parkingResponse.Value);
            return route;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get route");
            return Errors.Map.RouteNotFound();
        }
    }
}