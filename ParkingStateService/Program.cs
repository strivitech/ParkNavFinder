using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ParkingStateService.Database;
using ParkingStateService.Infrastructure;
using ParkingStateService.Kafka;
using ParkingStateService.Parking;
using ParkingStateService.SpatialIndex;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddControllers();

builder.AddNpgsqlDbContext<ParkingStateDbContext>("ParkingStateDb");

builder.Services.AddDbContextFactory<ParkingStateDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ParkingStateDb")));

builder.Services.AddKafkaBroker(builder.Configuration);

builder.Services.AddHangfire(cong => cong
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(conf =>
        conf.UseNpgsqlConnection(builder.Configuration.GetConnectionString("ParkingStateDb"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IGeoIndicesRetrieverService, GeoIndicesRetrieverService>();
builder.Services.AddScoped<IGeoIndexStateNotificationService, GeoIndexStateNotificationService>();
builder.Services.AddScoped<IGeoIndexStateEventPublisher, GeoIndexStateEventPublisher>();
builder.Services.AddScoped<IParkingStateProvider, ParkingStateProvider>();

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

try
{
    if (app.Environment.IsDevelopment())
    {
        app.EnsureDbCreated();
    }

    app.UseHangfireDashboard();

    RecurringJob.AddOrUpdate<GeoIndexStateNotificationJob>(
        recurringJobId: nameof(GeoIndexStateNotificationJob),
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