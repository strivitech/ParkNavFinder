using System.Collections.Immutable;
using Auth.Shared;
using DataManager.Api.Common;
using DataManager.Api.Contracts;
using DataManager.Api.Domain;
using Microsoft.AspNetCore.SignalR.Client;

namespace DataManager.Api.Services;

public class UserCoordinatesSenderService(
    ILogger<UserCoordinatesSenderService> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly ILogger<UserCoordinatesSenderService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000, stoppingToken);
        using var scope = _serviceProvider.CreateScope();
        var userWsConnections = scope.ServiceProvider.GetRequiredService<IUserWebSocketConnectionBuilder>();
        var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
        var routeGenerator = scope.ServiceProvider.GetRequiredService<IRouteGenerator>();
        var users = await userManager.GetGeneratedUsersAsync(Roles.User);
        var connections = users.ToImmutableDictionary(
            user => user.UserId,
            user => userWsConnections.BuildConnection(user.UserId));

        var startConnectionTasks = connections
            .Select(connection => connection.Value.StartAsync(stoppingToken));
        await Task.WhenAll(startConnectionTasks);

        var routes = await Task.WhenAll(users.Select(_ => routeGenerator.GenerateRouteAsync()));

        if (routes.Length != users.Count)
        {
            throw new InvalidOperationException("Number of routes does not match number of users");
        }
        
        Dictionary<string, Driver> userIdToDrivers = [];
        for (int i = 0; i < users.Count; i++)
        {
            userIdToDrivers[users[i].UserId] = new Driver(routes[i]);
        }

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var sendTasks = userIdToDrivers
                .Select(async kp =>
                {
                    var path = kp.Value.MoveToNextPosition(0.1);
                    if (path.Count == 0)
                    {
                        return;
                    }
                    
                    await connections[kp.Key].SendAsync("SendLocation", path, cancellationToken: stoppingToken);
                });
            await Task.WhenAll(sendTasks);
        }

        var disposeConnectionTasks = connections.Values
            .Select(connection => connection.DisposeAsync().AsTask());

        await Task.WhenAll(disposeConnectionTasks);
    }
}