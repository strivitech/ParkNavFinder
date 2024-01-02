using Auth.Shared;
using ParkingManagementService.Common;
using ParkingManagementService.Configuration;
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

builder.AddNpgsqlDbContext<ParkingDbContext>("parkingsdb");
builder.Services.AddKafkaBroker(builder.Configuration);

builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddScoped<IParkingServiceEventPublisher, ParkingServiceEventPublisher>();

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

app.Run();
