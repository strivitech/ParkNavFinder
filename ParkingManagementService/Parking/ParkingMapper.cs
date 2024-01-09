using Riok.Mapperly.Abstractions;

namespace ParkingManagementService.Parking;

[Mapper]
public partial class ParkingMapper
{
    public partial ParkingModel ToEntity(AddParkingRequest request);
    
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Id))]
    public partial void ToEntity(UpdateParkingRequest request, ParkingModel parkingModel);
    
    public partial GetParkingResponse ToGetParkingResponse(ParkingModel parkingModel);
}