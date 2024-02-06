using Auth.Shared;
using Polly;
using Polly.Contrib.WaitAndRetry;
using User.NotificationService.Common;
using User.NotificationService.Configurations;
using User.NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHttpClient<IUserLocationService, UserLocationService>(
        client =>
        {
            client.BaseAddress = new Uri("http://UserLocationService/");
            client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
                builder.Configuration[ApiKeyConstants.OwnApiKeyName]);
        })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(RequestPolly.MedianFirstRetryDelaySeconds),
                RequestPolly.DefaultRetryCount)));

builder.Services.AddHttpClient<IWebsocketManager, WebsocketManager>(
        client =>
        {
            client.BaseAddress = new Uri("http://WebSocketManager/");
            client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName,
                builder.Configuration[ApiKeyConstants.OwnApiKeyName]);
        })
    .AddTransientHttpErrorPolicy(
        policyBuilder => policyBuilder.WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(RequestPolly.MedianFirstRetryDelaySeconds),
                RequestPolly.DefaultRetryCount)));

builder.Services.AddControllers();

builder.Services.AddKafkaBroker(builder.Configuration);

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

app.MapControllers();

app.Run();