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
        // await Task.Delay(5000, stoppingToken);
        try
        {
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

                        const double maxJumpKilometers = 0.05;
                        var coordinate = kp.Value.MoveToNextPosition(maxJumpKilometers);

                        await connections[kp.Key]
                            .SendAsync("SendLocation", coordinate, cancellationToken: stoppingToken);
                        await _hubContext.Clients.All.ReceiveDriverLocation(kp.Key, coordinate);
                    });
                await Task.WhenAll(sendTasks);
            }

            var disposeConnectionTasks = connections.Values
                .Select(connection => connection.DisposeAsync().AsTask());

            await Task.WhenAll(disposeConnectionTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}