using System.Reflection;
using Auth.Shared;
using FluentValidation;
using ParkingManagementService.Common;
using ParkingManagementService.Infrastructure;
using ParkingManagementService.Kafka;
using ParkingManagementService.Parking;
using ParkingManagementService.User;
using ParkingManagementService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddControllers();

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
