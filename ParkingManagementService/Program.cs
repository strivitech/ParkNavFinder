using System.Reflection;
using Auth.Shared;
using FluentValidation;
using ParkingManagementService.Broker;
using ParkingManagementService.Common;
using ParkingManagementService.Database;
using ParkingManagementService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddControllers();

builder.Services.AddSharedAuth(new AuthConfig
{
    Authority = builder.Configuration["Auth0:Authority"]!,
    Audience = builder.Configuration["Auth0:Audience"]!
});

builder.AddNpgsqlDbContext<ParkingDbContext>("parkingsdb");
builder.Services.AddKafkaBroker(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IUserSessionData, CurrentUserSessionData>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddScoped<IParkingServiceEventPublisher, ParkingServiceEventPublisher>();
builder.Services.AddScoped<IModelValidationService, ModelValidationService>();

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
