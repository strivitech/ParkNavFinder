using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var userWebSocketHandler = builder.AddProject<Projects.User_WebSocketHandler>("UserWebSocketHandler");

var webSocketManagerRedis = builder
    .AddRedisContainer(
        name: "WebSocketManagerRedis",
        port: builder.Configuration.GetValue<int>("WebSocketManagerRedis:port"));
var webSocketManager = builder.AddProject<Projects.WebSocketManager>("WebSocketManager");

var mapService = builder.AddProject<Projects.MapService>("MapService");

userWebSocketHandler
    .WithReference(webSocketManager);

webSocketManager.WithReference(webSocketManagerRedis);

builder.AddProject<Projects.Yarp_ApiGateway>("YarpApiGateway")
    .WithReference(userWebSocketHandler)
    .WithLaunchProfile("https");

var userActiveGeoIndexRedis = builder
    .AddRedisContainer(
        name: "UserActiveGeoIndexRedis",
        port: builder.Configuration.GetValue<int>("UserActiveGeoIndexRedis:port"));

builder.AddProject<Projects.UserActiveGeoIndexService>("UserActiveGeoIndexService")
    .WithReference(userActiveGeoIndexRedis)
    .WithReference(mapService);

var parkingManagementPostgres = builder
    .AddPostgresContainer(
        name: "ParkingManagementPostgres", 
        port: builder.Configuration.GetValue<int>("ParkingDb:port"),
        password: builder.Configuration.GetValue<string>("ParkingDb:password"))
    .AddDatabase("ParkingDb");

var parkingManagementRedis = builder
    .AddRedisContainer(
        name: "ParkingManagementRedis",
        port: builder.Configuration.GetValue<int>("ParkingManagementRedis:port"));

builder.AddProject<Projects.ParkingManagementService>("ParkingManagementService")
    .WithReference(parkingManagementPostgres)
    .WithReference(parkingManagementRedis);

var parkingStatePostgres = builder
    .AddPostgresContainer(
        name: "ParkingStatePostgres", 
        port: builder.Configuration.GetValue<int>("ParkingStateDb:port"),
        password: builder.Configuration.GetValue<string>("ParkingStateDb:password"))
    .AddDatabase("ParkingStateDb");
builder.AddProject<Projects.ParkingStateService>("ParkingStateService")
    .WithReference(parkingStatePostgres);

builder.Build().Run();