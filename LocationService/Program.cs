using System.Reflection;
using FluentValidation;
using LocationService.Infrastructure;
using LocationService.Kafka;
using LocationService.UserLocation;
using LocationService.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddKafkaBroker(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IRequestValidator, RequestValidator>();
builder.Services.AddScoped<IUserLocationService, UserLocationService>();

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