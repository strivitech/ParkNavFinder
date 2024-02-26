using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public interface IParkingManager
{
    Task<List<GetParkingResponse>> GetAllByProviderAsync(string token);

    Task CreateAsync(CreateParkingRequest request, string token);
}