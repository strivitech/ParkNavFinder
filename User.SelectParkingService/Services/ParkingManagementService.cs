using ErrorOr;
using User.SelectParkingService.Common;
using User.SelectParkingService.Contracts;

namespace User.SelectParkingService.Services;

public class ParkingManagementService(HttpClient httpClient, ILogger<ParkingManagementService> logger)
    : IParkingManagementService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ParkingManagementService> _logger = logger;

    public async Task<ErrorOr<Coordinate>> GetParkingCoordsAsync(Guid parkingId)
    {
        if (parkingId == Guid.Empty)
        {
            return Error.Validation(nameof(parkingId), "ParkingId cannot be empty.");
        }

        try
        {
            var response = await _httpClient.GetAsync($"api/parking/{parkingId}");
            response.EnsureSuccessStatusCode();
            
            var parkingCoords = await response.Content.ReadFromJsonAsync<GetParkingCoordinatesResponse>();
            if (parkingCoords is null)
            {
                return Errors.Parking.NotFound(parkingId);
            }
            
            return new Coordinate(parkingCoords.Latitude, parkingCoords.Longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get parking coordinates");
            return Errors.Parking.NotFound(parkingId);
        }
    }
}