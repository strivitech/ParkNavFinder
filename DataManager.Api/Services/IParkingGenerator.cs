using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IParkingGenerator
{
    IEnumerable<CreateParkingRequest> GenerateRealisticParkingData(int count);
}