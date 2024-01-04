using ErrorOr;
using ParkingManagementService.Requests;
using ParkingManagementService.Responses;

namespace ParkingManagementService.Services;

public interface IParkingService
{
    Task<ErrorOr<Created>> AddAsync(AddParkingRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(UpdateParkingRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(DeleteParkingRequest request);
    Task<ErrorOr<GetParkingResponse>> GetAsync(GetParkingRequest request);  
}