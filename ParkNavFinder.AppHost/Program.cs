var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UserWsHandler>("userwshandler");

builder.Build().Run();
