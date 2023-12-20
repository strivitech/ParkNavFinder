using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Shared;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute(string apiKeyName) : Attribute, IAuthorizationFilter
{
    private readonly string _apiKeyName = apiKeyName;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? apiKey = context.HttpContext.Request.Headers[ApiKeyConstants.HeaderPrefix + _apiKeyName];
        
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        
        if (!IsApiKeyValid(configuration, apiKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool IsApiKeyValid(IConfiguration configuration, string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {   
            return false;
        }

        var apiKeyFromConfig = configuration
            .GetValue<string>($"{ApiKeyConstants.ConfigSection}:{ApiKeyConstants.ParkingOfferingService}");
        
        return apiKey == apiKeyFromConfig;
    }
}