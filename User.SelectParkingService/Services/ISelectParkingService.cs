using ErrorOr;
using User.SelectParkingService.Contracts;
using Route = User.SelectParkingService.Contracts.Route;

namespace User.SelectParkingService.Services;

public interface ISelectParkingService
{
    Task<ErrorOr<Route>> SelectParkingAsync(SelectParkingRequest request);
}