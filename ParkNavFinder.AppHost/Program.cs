var builder = DistributedApplication.CreateBuilder(args);

var userRedis = builder.AddRedisContainer("userredis");
builder.AddProject<Projects.UserService>("userservice")
    .WithReference(userRedis);

builder.Build().Run();
