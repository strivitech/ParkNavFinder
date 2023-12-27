var builder = DistributedApplication.CreateBuilder(args);

var userWsHandler = builder.AddProject<Projects.UserWsHandler>("userwshandler");

var webSocketManagerRedis = builder.AddRedisContainer("webSocketManagerRedis");
var webSocketManager = builder.AddProject<Projects.WebSocketManager>("websocketmanager");

var locationService = builder.AddProject<Projects.LocationService>("locationservice");

builder.AddProject<Projects.MapService>("mapservice");

userWsHandler
    .WithReference(webSocketManager)
    .WithReference(locationService);

webSocketManager
    .WithReference(userWsHandler)
    .WithReference(webSocketManagerRedis);

builder.Build().Run();
