using Auth.Shared;
using Polly;
using Polly.Contrib.WaitAndRetry;
using User.SelectParkingService.Common;
using User.SelectParkingService.Database;
using User.SelectParkingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.AddNpgsqlDbContext<ParkingSelectionDbContext>("UserSelectingParkingDb");

builder.Services.AddHttpContextAccessor();

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

builder.Services.AddHttpClient<IParkingManagementService, ParkingManagementService>(
        client =>
        {
            client.BaseAddress = new Uri("http://ParkingManagementService/");
            client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
                builder.Configuration[ApiKeyConstants.OwnApiKeyName]);
        })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(RequestPolly.MedianFirstRetryDelaySeconds),
                RequestPolly.DefaultRetryCount)));

builder.Services.AddScoped<IUserSessionData, CurrentUserSessionData>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISelectParkingService, SelectParkingService>();

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
