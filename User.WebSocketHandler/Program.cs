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

builder.Services.AddCustomAuth(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(config =>
    config.AddPolicy(
        "AllowedOrigins",
        p => p.WithOrigins(builder.Configuration["AllowedCorsOrigins"]!.Split(','))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

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

app.UseCors("AllowedOrigins");

app.UseSharedAuth();

app.MapControllers();
app.MapHub<UsersHub>("/api/usershub");

app.Run();
