using System.Net.Http.Headers;
using DataManager.Api.Contracts;
using DataManager.Api.Validation;

namespace DataManager.Api.Services;

public class ParkingManager(IRequestValidator requestValidator, HttpClient httpClient) : IParkingManager
{
    private readonly IRequestValidator _requestValidator = requestValidator;
    private readonly HttpClient _httpClient = httpClient;
    
    public async Task<List<GetParkingResponse>> GetAllByProviderAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.GetFromJsonAsync<List<GetParkingResponse>>("api/parking");
        return response ?? [];
    }

    public async Task CreateAsync(CreateParkingRequest request, string token)
    {
        _requestValidator.ThrowIfNotValid(request);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.PostAsJsonAsync("api/parking", request);
        response.EnsureSuccessStatusCode();
    }
}