using Auth.Shared;
using Polly;
using Polly.Contrib.WaitAndRetry;
using UserActiveGeoIndexService.Common;
using UserActiveGeoIndexService.Common.Configuration;
using UserActiveGeoIndexService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.AddRedis("UserActiveGeoIndexRedis");

builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection(KafkaConfig.SectionName));

builder.Services.AddKafkaBroker(builder.Configuration);

builder.Services.AddHttpClient<IGeoIndexService, GeoIndexService>(
        client =>
        {
            client.BaseAddress = new Uri("http://MapService/");
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

app.UseExceptionHandler(_ => { });

app.UseHttpsRedirection();

app.MapControllers();

app.Run();