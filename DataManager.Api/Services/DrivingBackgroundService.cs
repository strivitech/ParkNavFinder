using System.Collections.Immutable;
using Auth.Shared;
using DataManager.Api.Common;
using DataManager.Api.Contracts;
using DataManager.Api.Domain;
using DataManager.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace DataManager.Api.Services;

public class DrivingBackgroundService(
    ILogger<DrivingBackgroundService> logger,
    IServiceProvider serviceProvider,
    IHubContext<DriversHub, IDriversClient> hubContext)
    : BackgroundService
{
    private readonly ILogger<DrivingBackgroundService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IHubContext<DriversHub, IDriversClient> _hubContext = hubContext;
    private const double CoordsUpdatePeriodInSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var userWsConnections = scope.ServiceProvider.GetRequiredService<IUserWebSocketConnectionBuilder>();
            var userManager = scope.ServiceProvider.GetRequiredService<IUserManager>();
            var routeGenerator = scope.ServiceProvider.GetRequiredService<IRouteGenerator>();
            var users = await userManager.GetGeneratedUsersAsync(Roles.User);
            
            var connections = await InitializeConnections(users, userWsConnections, stoppingToken);

            var routes = await GenerateRoutesAsync(users, routeGenerator);
            var userIdToDrivers = MapUserIdToDrivers(users, routes);

            await ProcessDriverUpdatesAsync(connections, userIdToDrivers, stoppingToken);
            
            await DisposeConnectionsAsync(connections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while driving");
            throw;
        }
    }

    private static async Task<ImmutableDictionary<string, HubConnection>> InitializeConnections(List<GetUserResponse> users,
        IUserWebSocketConnectionBuilder userWsConnections, CancellationToken stoppingToken)
    {
        var connections = users.ToImmutableDictionary(
            user => user.UserId,
            user => userWsConnections.BuildConnection(user.UserId));

        var startConnectionTasks = connections
            .Select(connection => connection.Value.StartAsync(stoppingToken));
        await Task.WhenAll(startConnectionTasks);
        return connections;
    }

    private static Dictionary<string, Driver> MapUserIdToDrivers(List<GetUserResponse> users, Route[] routes)
    {
        Dictionary<string, Driver> userIdToDrivers = [];
        for (int i = 0; i < users.Count; i++)
        {
            userIdToDrivers[users[i].UserId] = new Driver(routes[i]);
        }

        return userIdToDrivers;
    }

    private static async Task<Route[]> GenerateRoutesAsync(List<GetUserResponse> users, IRouteGenerator routeGenerator)
    {
        var routes = await Task.WhenAll(users.Select(_ => routeGenerator.GenerateRouteAsync()));

        if (routes.Length != users.Count)
        {
            throw new InvalidOperationException("Number of routes does not match number of users");
        }

        return routes;
    }

    private static async Task DisposeConnectionsAsync(ImmutableDictionary<string, HubConnection> connections)
    {
        var disposeConnectionTasks = connections.Values
            .Select(connection => connection.DisposeAsync().AsTask());

        await Task.WhenAll(disposeConnectionTasks);
    }

    private async Task ProcessDriverUpdatesAsync(
        ImmutableDictionary<string, HubConnection> connections,
        Dictionary<string, Driver> userIdToDrivers,
        CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(CoordsUpdatePeriodInSeconds));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var sendTasks = userIdToDrivers
                .Select(async kp =>
                {
                    if (kp.Value.IsAtDestination)
                    {
                        return;
                    }

                    const double maxJumpKilometers = 0.07;
                    var coordinate = kp.Value.MoveToNextPosition(maxJumpKilometers);

                    await connections[kp.Key]
                        .SendAsync("SendLocation", coordinate, cancellationToken: stoppingToken);
                    await _hubContext.Clients.All.ReceiveDriverLocation(kp.Key, coordinate);
                });
            await Task.WhenAll(sendTasks);
        }
    }
}