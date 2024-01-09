using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ParkingStateService.Broker;
using ParkingStateService.Common;
using ParkingStateService.Database;
using ParkingStateService.Jobs;
using ParkingStateService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddScoped<IParkingIndicesRetrieverService, ParkingIndicesRetrieverService>();
builder.Services.AddScoped<IIndexStateNotificationService, IndexStateNotificationService>();
builder.Services.AddScoped<IIndexStateEventPublisher, IndexStateEventPublisher>();
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

    RecurringJob.AddOrUpdate<ParkingStateNotificationJob>(
        recurringJobId: nameof(ParkingStateNotificationJob),
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