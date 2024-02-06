namespace User.LocationService.Services;

public interface IIndicesService
{
    Task<List<string>> GetUsersAttachedToIndexAsync(string index);
}