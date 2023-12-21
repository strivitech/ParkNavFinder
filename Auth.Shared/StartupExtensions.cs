using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Shared;

public static class StartupExtensions
{
    public static IServiceCollection AddSharedAuth(this IServiceCollection services, AuthConfig config)
    {   
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = config.Authority;
                options.Audience = config.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = "http://parknavfinder.com/roles"
                };
            });
        
        return services;    
    }
    
    public static WebApplication UseSharedAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}