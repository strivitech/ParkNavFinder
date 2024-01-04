using System.Reflection;
using FluentValidation;
using LocationService.Common;
using LocationService.Common.Configuration;
using LocationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddKafkaBroker(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IModelValidationService, ModelValidationService>();
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