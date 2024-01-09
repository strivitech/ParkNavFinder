using Riok.Mapperly.Abstractions;

namespace ParkingManagementService.Parking;

[Mapper]
public partial class ParkingMapper
{
    public partial Parking ToEntity(AddParkingRequest request);
    
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Id))]
    public partial void ToEntity(UpdateParkingRequest request, Parking parking);
    
    public partial GetParkingResponse ToGetParkingResponse(Parking parking);
}