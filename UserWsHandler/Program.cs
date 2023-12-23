using Auth.Shared;
using Polly;
using Polly.Contrib.WaitAndRetry;
using UserWsHandler.Common;
using UserWsHandler.Hubs;
using UserWsHandler.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddScoped<ILocationService, LocationService>();

builder.Services.AddHttpClient<IWsManagerService, WsManagerService>(
    client =>
    {
        client.BaseAddress = new Uri("http://websocketmanager/");
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

app.UseHttpsRedirection();

app.UseSharedAuth();

app.MapControllers();
app.MapHub<UsersHub>("/usershub");

app.Run();
