using ErrorOr;
using User.SelectParkingService.Contracts;

namespace User.SelectParkingService.Services;

public interface IParkingManagementService
{
    Task<ErrorOr<Coordinate>> GetParkingCoordsAsync(Guid parkingId);
}