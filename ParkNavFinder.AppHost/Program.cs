var builder = DistributedApplication.CreateBuilder(args);

var userWsHandler = builder.AddProject<Projects.UserWsHandler>("userwshandler");

var webSocketManagerRedis = builder.AddRedisContainer("webSocketManagerRedis");
var webSocketManager = builder.AddProject<Projects.WebSocketManager>("websocketmanager");

userWsHandler.WithReference(webSocketManager);

webSocketManager
    .WithReference(userWsHandler)
    .WithReference(webSocketManagerRedis);

builder.AddProject<Projects.MapService>("mapservice");

builder.Build().Run();
