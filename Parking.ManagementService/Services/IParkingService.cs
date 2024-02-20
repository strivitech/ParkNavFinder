using ErrorOr;
using Parking.ManagementService.Contracts;

namespace Parking.ManagementService.Services;

public interface IParkingService
{
    Task<ErrorOr<Created>> AddAsync(AddParkingRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(DeleteParkingRequest request);
    Task<ErrorOr<GetParkingResponse>> GetAsync(GetParkingRequest request);
    Task<ErrorOr<List<GetParkingResponse>>> GetAllByProviderAsync();
}