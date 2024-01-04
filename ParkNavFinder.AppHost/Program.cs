using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var userWsHandler = builder.AddProject<Projects.UserWsHandler>("userwshandler");

var webSocketManagerRedis = builder
    .AddRedisContainer(
        name: "webSocketManagerRedis",
        port: builder.Configuration.GetValue<int>("webSocketManagerRedis:port"));
var webSocketManager = builder.AddProject<Projects.WebSocketManager>("websocketmanager");

var locationService = builder.AddProject<Projects.LocationService>("locationservice");

var mapService = builder.AddProject<Projects.MapService>("mapservice");

userWsHandler
    .WithReference(webSocketManager)
    .WithReference(locationService);

webSocketManager.WithReference(webSocketManagerRedis);

builder.AddProject<Projects.Yarp_ApiGateway>("yarpapigateway")
    .WithReference(userWsHandler)
    .WithLaunchProfile("https");

var userActiveGeoIndexRedis = builder
    .AddRedisContainer(
        name: "userActiveGeoIndexRedis",
        port: builder.Configuration.GetValue<int>("userActiveGeoIndexRedis:port"));

builder.AddProject<Projects.UserActiveGeoIndexService>("useractivegeoindexservice")
    .WithReference(userActiveGeoIndexRedis)
    .WithReference(mapService);

var parkingManagementPostgres = builder
    .AddPostgresContainer(
        name: "parkingManagementPostgres", 
        port: builder.Configuration.GetValue<int>("parkingsdb:port"),
        password: builder.Configuration.GetValue<string>("parkingsdb:password"))
    .AddDatabase("parkingsdb");

var parkingManagementRedis = builder
    .AddRedisContainer(
        name: "parkingManagementRedis",
        port: builder.Configuration.GetValue<int>("parkingManagementRedis:port"));

builder.AddProject<Projects.ParkingManagementService>("parkingmanagementservice")
    .WithReference(parkingManagementPostgres)
    .WithReference(parkingManagementRedis);

builder.Build().Run();