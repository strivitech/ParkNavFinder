using ErrorOr;
using ParkingManagementService.Requests;

namespace ParkingManagementService.Services;

public interface IParkingService
{
    Task<ErrorOr<Created>> AddAsync(AddParkingRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(DeleteParkingRequest request);
}