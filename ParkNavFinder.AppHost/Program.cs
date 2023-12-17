var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UserService>("userservice");

builder.Build().Run();
