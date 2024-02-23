namespace DataManager.Api.Services;

public interface IRouteGenerator
{
    Task<Route> GenerateRouteAsync();
}