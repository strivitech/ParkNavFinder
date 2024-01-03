using ParkingManagementService.Models;
using ParkingManagementService.Requests;
using ParkingManagementService.Responses;
using Riok.Mapperly.Abstractions;

namespace ParkingManagementService.Mappers;

[Mapper]
public partial class ParkingMapper
{
    public partial Parking ToEntity(AddParkingRequest request);
    
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Id))]
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Latitude))]
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Longitude))]
    public partial void ToEntity(UpdateParkingRequest request, Parking parking);
    
    public partial GetParkingResponse ToGetParkingResponse(Parking parking);
}