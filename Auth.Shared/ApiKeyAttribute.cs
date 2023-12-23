using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Shared;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _apiKeyNames;

    public ApiKeyAttribute(params string[] apiKeyNames)
    {
        if (apiKeyNames is null || apiKeyNames.Length == 0)
        {
            throw new ArgumentException("You must provide at least one API key name", nameof(apiKeyNames));
        }
        
        _apiKeyNames = apiKeyNames;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out var actualApiKey);

        if (string.IsNullOrWhiteSpace(actualApiKey))
        {
            context.Result = new UnauthorizedResult();
        }
        
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        if (!IsApiKeyValid(configuration,actualApiKey!))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsApiKeyValid(IConfiguration configuration, string actualApiKey)
    {   
        var configSection = configuration.GetRequiredSection(ApiKeyConstants.ConfigSectionName);
        
        return _apiKeyNames.Any(apiKeyName =>
            configSection.GetValue<string>(apiKeyName) == actualApiKey);
    }
}