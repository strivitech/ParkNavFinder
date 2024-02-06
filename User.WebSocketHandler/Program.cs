using System.Reflection;
using Auth.Shared;
using FluentValidation;
using Polly;
using Polly.Contrib.WaitAndRetry;
using User.WebSocketHandler.Common;
using User.WebSocketHandler.Configurations;
using User.WebSocketHandler.Hubs;
using User.WebSocketHandler.Services;
using User.WebSocketHandler.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddSharedAuth(new AuthConfig
{
    Authority = builder.Configuration["Auth0:Authority"]!,
    Audience = builder.Configuration["Auth0:Audience"]!
});

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddKafkaBroker(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IRequestValidator, RequestValidator>();
builder.Services.AddScoped<IUserLocationService, UserLocationService>();

builder.Services.AddHttpClient<IWsManagerService, WsManagerService>(
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

builder.Services.AddScoped<IIndexStateNotificationService, IndexStateNotificationService>();

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

app.UseSharedAuth();

app.MapControllers();
app.MapHub<UsersHub>("/api/usershub");

app.Run();
