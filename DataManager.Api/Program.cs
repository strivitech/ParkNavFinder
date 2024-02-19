using System.Reflection;
using Auth0.ManagementApi;
using DataManager.Api.Common;
using DataManager.Api.Services;
using DataManager.Api.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ManagementApiClient>(_ => new ManagementApiClient(
    builder.Configuration["Auth0:ManagementApiToken"],
    new Uri($"{builder.Configuration["Auth0:Authority"]}api/v2")));

builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();

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

