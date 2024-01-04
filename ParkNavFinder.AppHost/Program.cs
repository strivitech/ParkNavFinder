var builder = DistributedApplication.CreateBuilder(args);

var userWsHandler = builder.AddProject<Projects.UserWsHandler>("userwshandler");

var webSocketManagerRedis = builder.AddRedisContainer("webSocketManagerRedis");
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

var userActiveGeoIndexRedis = builder.AddRedisContainer("userActiveGeoIndexRedis");

builder.AddProject<Projects.UserActiveGeoIndexService>("useractivegeoindexservice")
    .WithReference(userActiveGeoIndexRedis)
    .WithReference(mapService);

var parkingManagementServicePostgres = builder
    .AddPostgresContainer("parkingManagementPostgres", port: 8001, password: "admin")
    .AddDatabase("parkingsdb");

builder.AddProject<Projects.ParkingManagementService>("parkingmanagementservice")
    .WithReference(parkingManagementServicePostgres);

builder.Build().Run();