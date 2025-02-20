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

var userLocationRedis = builder
    .AddRedisContainer( 
        name: "UserLocationRedis",
        port: builder.Configuration.GetValue<int>("UserLocationRedis:port"));

var userLocationService = builder.AddProject<Projects.User_LocationService>("UserLocationService");
    
userLocationService
    .WithReference(userLocationRedis)
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

var parkingManagementService = builder.AddProject<Projects.Parking_ManagementService>("ParkingManagementService")
    .WithReference(parkingManagementPostgres)
    .WithReference(parkingManagementRedis)
    .WithReference(mapService);

var parkingStatePostgres = builder
    .AddPostgresContainer(
        name: "ParkingStatePostgres", 
        port: builder.Configuration.GetValue<int>("ParkingStateDb:port"),
        password: builder.Configuration.GetValue<string>("ParkingStateDb:password"))
    .AddDatabase("ParkingStateDb");

builder.AddProject<Projects.Parking_StateService>("ParkingStateService")
    .WithReference(parkingStatePostgres);

builder.AddProject<Projects.User_NotificationService>("UserNotificationService")
    .WithReference(userLocationService)
    .WithReference(webSocketManager)
    .WithReference(userWebSocketHandler);

var dataManagerApi = builder.AddProject<Projects.DataManager_Api>("DataManagerApi")
    .WithReference(parkingManagementService)
    .WithReference(userWebSocketHandler)
    .WithReference(mapService);

builder.AddNpmApp("DataManagerVue", "../Clients/datamanager.client", "serve")
    .WithReference(dataManagerApi)
    .WithEndpoint(containerPort: builder.Configuration.GetValue<int>("DataManagerVue:port"), scheme: "https")
    .AsDockerfileInManifest();
    
var parkingAnalyticsPostgres = builder
    .AddPostgresContainer(
        name: "ParkingAnalyticsPostgres", 
        port: builder.Configuration.GetValue<int>("ParkingAnalyticsDb:port"),
        password: builder.Configuration.GetValue<string>("ParkingAnalyticsDb:password"))
    .AddDatabase("ParkingAnalyticsDb");

builder.AddProject<Projects.Parking_AnalyticsService>("ParkingAnalyticsService")
    .WithReference(parkingAnalyticsPostgres)
    .WithReference(userLocationService);

var userSelectingParkingDb = builder
    .AddPostgresContainer(
        name: "UserSelectingParkingPostgres", 
        port: builder.Configuration.GetValue<int>("UserSelectingParkingDb:port"),
        password: builder.Configuration.GetValue<string>("UserSelectingParkingDb:password"))
    .AddDatabase("UserSelectingParkingDb");

var userSelectParkingService = builder.AddProject<Projects.User_SelectParkingService>("UserSelectParkingService")
    .WithReference(mapService)
    .WithReference(parkingManagementService)
    .WithReference(userSelectingParkingDb);

builder.AddNpmApp("UserAppVue", "../Clients/user-app", "serve")
    .WithReference(userWebSocketHandler)
    .WithReference(parkingManagementService)
    .WithReference(userSelectParkingService)
    .WithEndpoint(containerPort: builder.Configuration.GetValue<int>("UserAppVue:port"), scheme: "https")
    .AsDockerfileInManifest();

builder.Build().Run();