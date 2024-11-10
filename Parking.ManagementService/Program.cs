using System.Reflection;
using Auth.Shared;
using FluentValidation;
using Parking.ManagementService.Common;
using Parking.ManagementService.Configurations;
using Parking.ManagementService.Database;
using Parking.ManagementService.Services;
using Parking.ManagementService.Validation;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddControllers();

builder.Services.AddCors(config =>
    config.AddPolicy(
        "AllowedOrigins",
        p => p.WithOrigins(builder.Configuration["AllowedCorsOrigins"]!.Split(','))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

builder.Services.AddSharedAuth(new AuthConfig
{
    Authority = builder.Configuration["Auth0:Authority"]!,
    Audience = builder.Configuration["Auth0:Audience"]!
});

builder.AddNpgsqlDbContext<ParkingDbContext>("ParkingDb");
builder.Services.AddKafkaBroker(builder.Configuration);
builder.AddRedis("ParkingManagementRedis");

builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IUserSessionData, CurrentUserSessionData>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.Decorate<IParkingService, CachingParkingService>();
builder.Services.AddScoped<IParkingServiceEventPublisher, ParkingServiceEventPublisher>();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();

builder.Services.AddHttpClient<IMapService, MapService>(
        client =>
        {
            client.BaseAddress = new Uri("http://MapService/");
            client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
                builder.Configuration[ApiKeyConstants.OwnApiKeyName]);
        })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(RequestPolly.MedianFirstRetryDelaySeconds),
                RequestPolly.DefaultRetryCount)));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.UseCors("AllowedOrigins");

app.UseSharedAuth();

app.MapControllers();

try
{
    if (app.Environment.IsDevelopment())
    {
        app.EnsureDbCreated();
    }
    app.Run();
}
catch (Exception ex)
{
    // Log the exception
    throw;
}
finally
{
    
}
