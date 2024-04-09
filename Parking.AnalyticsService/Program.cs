using Auth.Shared;
using Hangfire;
using Hangfire.PostgreSql;
using Parking.AnalyticsService.Common;
using Parking.AnalyticsService.Configurations;
using Parking.AnalyticsService.Database;
using Parking.AnalyticsService.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaBroker(builder.Configuration);

builder.AddNpgsqlDbContext<ParkingDbContext>("ParkingAnalyticsDb");

builder.Services.AddHangfire(cong => cong
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(conf =>
        conf.UseNpgsqlConnection(builder.Configuration.GetConnectionString("ParkingAnalyticsDb"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IUserLocationService, UserLocationService>();
builder.Services.AddScoped<IParkingRetrieverService, ParkingRetrieverService>();
builder.Services.AddScoped<IParkingStatesCalculator, ParkingStatesCalculator>();

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

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

try
{
    if (app.Environment.IsDevelopment())
    {
        app.EnsureDbCreated();
    }

    app.UseHangfireDashboard();

    RecurringJob.AddOrUpdate<ParkingStateChangerService>(
        recurringJobId: nameof(ParkingStateChangerService),
        methodCall: x => x.Complete(),
        cronExpression: "*/5 * * * *");

    app.MapHangfireDashboard();

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