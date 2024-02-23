using System.Reflection;
using Auth.Shared;
using Auth0.ManagementApi;
using DataManager.Api.Common;
using DataManager.Api.Services;
using DataManager.Api.Validation;
using FluentValidation;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ManagementApiClient>(_ => new ManagementApiClient(
    builder.Configuration["Auth0:ManagementApiToken"],
    new Uri($"{builder.Configuration["Auth0:Authority"]}api/v2")));

builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.Decorate<IUserManager, CachingUserManager>();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();
builder.Services.AddScoped<IUserGenerator, UserGenerator>();
// builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IEmailGenerator, GmailSubEmailsGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IParkingManager, ParkingManager>();
builder.Services.AddSingleton<ITokenStorage, InMemoryAccessTokenStorage>();
builder.Services.AddScoped<IParkingGenerator, ParkingGenerator>();
builder.Services.AddScoped<IUserWebSocketConnectionBuilder, UserWebSocketConnectionBuilder>();
builder.Services.AddScoped<IRouteCreator, RouteCreator>();
builder.Services.AddScoped<IRouteGenerator, RouteGenerator>();
builder.Services.AddScoped<IMapService, MapService>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddHttpClient<IParkingManager, ParkingManager>(
        client => { client.BaseAddress = new Uri("http://ParkingManagementService/"); })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(200),
                1)));

builder.Services.AddHttpClient<IMapService, MapService>(
        client =>
        {
            client.BaseAddress = new Uri("http://MapService/");
            client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
                builder.Configuration[ApiKeyConstants.OwnApiKeyName]);
        })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(200),
                1)));

builder.Services.AddHostedService<UserCoordinatesSenderService>();

builder.Services.AddControllers();

builder.Services.AddCors(config =>
    config.AddPolicy(
        "AllowedOrigins",
        p => p.WithOrigins(builder.Configuration["AllowedCorsOrigins"]!.Split(','))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

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

app.MapControllers();

try
{
    app.EnsureUsersCreated();
    app.SetInMemoryTokensForUsers();
    app.EnsureParkingCreated();

    app.Run();
}
catch (Exception ex)
{
    // Add logs and flush
}