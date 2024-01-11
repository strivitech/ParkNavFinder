using Parking.ManagementService.Contracts;
using Riok.Mapperly.Abstractions;

namespace Parking.ManagementService.Mapping;

[Mapper]
public partial class ParkingMapper
{
    public partial Domain.Parking ToEntity(AddParkingRequest request);
    
    [MapperIgnoreSource(nameof(UpdateParkingRequest.Id))]
    public partial void ToEntity(UpdateParkingRequest request, Domain.Parking parking);
    
    public partial GetParkingResponse ToGetParkingResponse(Domain.Parking parking);
}