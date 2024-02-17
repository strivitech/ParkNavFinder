using ErrorOr;
using FluentAssertions;
using Kafka.Events.Contracts.Parking.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Parking.ManagementService.Common;
using Parking.ManagementService.Contracts;
using Parking.ManagementService.Database;
using Parking.ManagementService.Domain;
using Parking.ManagementService.Services;
using Parking.ManagementService.Validation;

namespace Parking.ManagementService.Tests;

public class ParkingServiceTests
{
    private readonly ParkingService _service;
    private readonly ParkingDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestValidator _requestValidator;
    private readonly IParkingServiceEventPublisher _eventPublisher;

    public ParkingServiceTests()
    {
        var options = new DbContextOptionsBuilder<ParkingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestParkingDb")
            .Options;
        _dbContext = Substitute.ForPartsOf<ParkingDbContext>(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        _eventPublisher = Substitute.For<IParkingServiceEventPublisher>();
        var logger = Substitute.For<ILogger<ParkingService>>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _requestValidator = Substitute.For<IRequestValidator>();

        _service = new ParkingService(_dbContext, _eventPublisher, logger, _currentUserService, _requestValidator);
    }

    [Fact]
    public async Task AddAsync_WhenValid_ReturnsCreated()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var request = CreateAddParkingRequest();
        _currentUserService.SessionData.Returns(defaultUserSessionData);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Created>();
        _dbContext.ParkingSet.Count().Should().Be(1);
        var parking = _dbContext.ParkingSet.First();
        parking.Name.Should().Be("Name");
        parking.Description.Should().Be("Description");
        parking.Address.Should().BeEquivalentTo(new Address("Country", "City", "Street", "123"));
    }

    [Fact]
    public async Task AddAsync_WhenValidationFails_ReturnsError()
    {
        // Arrange
        var request = CreateAddParkingRequest();
        var error = Error.Validation("Some error");
        _requestValidator.Validate(request).Returns([error]);

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(error);
    }

    [Fact]
    public async Task AddAsync_WhenDatabaseSaveFails_ReturnsError()
    {
        // Arrange
        var request = CreateAddParkingRequest();
        _dbContext.When(x => x.SaveChangesAsync()).Do(_ => throw new DbUpdateException());
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Code.Should().Be(Errors.Parking.AddFailed(Guid.Empty).Code);
        _dbContext.ParkingSet.Count().Should().Be(0);
    }

    [Fact]
    public async Task AddAsync_WhenNotPublishedParkingAddedEvent_ReturnsError()
    {   
        // Arrange
        var request = CreateAddParkingRequest();
        _requestValidator.Validate(request).Returns([]);
        _eventPublisher.PublishParkingAddedAsync(Arg.Any<ParkingAddedEvent>()).Returns(Error.Failure("Some error"));

        // Act
        var result = await _service.AddAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Code.Should().BeEquivalentTo(Errors.Parking.AddFailed(Guid.Empty).Code);
        _dbContext.ParkingSet.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenValid_ReturnsUpdated()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = CreateUpdateParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(defaultUserSessionData);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Updated>();
        _dbContext.ParkingSet.Count().Should().Be(1);
        var updatedParking = _dbContext.ParkingSet.First();
        updatedParking.Id.Should().Be(parkingId);
        updatedParking.Name.Should().Be("New Name");
        updatedParking.Description.Should().Be("New Description");
        updatedParking.Address.Should().BeEquivalentTo(new Address("New Country", "New City", "New Street", "456"));
    }

    [Fact]
    public async Task UpdateAsync_WhenDatabaseSaveFails_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = CreateUpdateParkingRequest(id);
        var defaultUserSessionData = new DefaultUserSessionData();
        var entity = CreateDefaultParkingEntity(id, defaultUserSessionData);
        _dbContext.ParkingSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.When(x => x.SaveChangesAsync()).Do(_ => throw new DbUpdateException());
        _requestValidator.Validate(request).Returns([]);
        _currentUserService.SessionData.Returns(defaultUserSessionData);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().Be(Errors.Parking.UpdateFailed(id));
        _dbContext.ParkingSet.Count().Should().Be(1);
        _dbContext.ParkingSet.First().Should().BeEquivalentTo(entity);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenValidationFails_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = CreateUpdateParkingRequest(id);
        var error = Error.Validation("Some error");
        _requestValidator.Validate(request).Returns([error]);
        
        // Act
        var result = await _service.UpdateAsync(request);
        
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(error);
    }

    [Fact]
    public async Task UpdateAsync_WhenParkingNotFound_ReturnsError()
    {
        // Arrange
        var request = CreateUpdateParkingRequest(Guid.NewGuid());
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(Errors.Parking.NotFound(request.Id));
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNotPublishedParkingUpdatedEvent_ReturnsError()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = CreateUpdateParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(defaultUserSessionData);
        _requestValidator.Validate(request).Returns([]);
        _eventPublisher.PublishParkingUpdatedAsync(Arg.Any<ParkingUpdatedEvent>()).Returns(Error.Failure("Some error"));

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Code.Should().BeEquivalentTo(Errors.Parking.UpdateFailed(request.Id).Code);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNotOwnedByCurrentUser_ReturnsError()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = CreateUpdateParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(new DefaultUserSessionData{UserId = "AnotherUserId"});
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(Errors.Parking.NotOwnedByCurrentUser(request.Id));
    }
    
    [Fact]
    public async Task DeleteAsync_WhenValid_ReturnsDeleted()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = new DeleteParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(defaultUserSessionData);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Deleted>();
        _dbContext.ParkingSet.Count().Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenDatabaseSaveFails_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new DeleteParkingRequest(id);
        var defaultUserSessionData = new DefaultUserSessionData();
        var entity = CreateDefaultParkingEntity(id, defaultUserSessionData);
        _dbContext.ParkingSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.When(x => x.SaveChangesAsync()).Do(_ => throw new DbUpdateException());
        _requestValidator.Validate(request).Returns([]);
        _currentUserService.SessionData.Returns(defaultUserSessionData);

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().Be(Errors.Parking.DeleteFailed(id));
        _dbContext.ParkingSet.Count().Should().Be(1);
        _dbContext.ParkingSet.First().Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task DeleteAsync_WhenValidationFails_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new DeleteParkingRequest(id);
        var error = Error.Validation("Some error");
        _requestValidator.Validate(request).Returns([error]);

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(error);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenParkingNotFound_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new DeleteParkingRequest(id);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(Errors.Parking.NotFound(request.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenNotPublishedParkingDeletedEvent_ReturnsError()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = new DeleteParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(defaultUserSessionData);
        _requestValidator.Validate(request).Returns([]);
        _eventPublisher.PublishParkingDeletedAsync(Arg.Any<ParkingDeletedEvent>()).Returns(Error.Failure("Some error"));

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Code.Should().BeEquivalentTo(Errors.Parking.DeleteFailed(request.Id).Code);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenNotOwnedByCurrentUser_ReturnsError()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = new DeleteParkingRequest(parkingId);
        _currentUserService.SessionData.Returns(new DefaultUserSessionData{UserId = "AnotherUserId"});
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.DeleteAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(Errors.Parking.NotOwnedByCurrentUser(request.Id));
        _dbContext.ParkingSet.Count().Should().Be(1);
        _dbContext.ParkingSet.First().Should().BeEquivalentTo(parking);
    }
    
    [Fact]
    public async Task GetAsync_WhenParkingExists_ReturnsGetParkingResponse()
    {
        // Arrange
        var defaultUserSessionData = new DefaultUserSessionData();
        var parkingId = Guid.NewGuid();
        var parking = CreateDefaultParkingEntity(parkingId, defaultUserSessionData);
        _dbContext.ParkingSet.Add(parking);
        await _dbContext.SaveChangesAsync();

        var request = new GetParkingRequest(parkingId);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.GetAsync(request);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<GetParkingResponse>();
        result.Value.Should().BeEquivalentTo(parking);
    }
    
    [Fact]
    public async Task GetAsync_WhenValidationFails_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetParkingRequest(id);
        var error = Error.Validation("Some error");
        _requestValidator.Validate(request).Returns([error]);

        // Act
        var result = await _service.GetAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(error);
    }
    
    [Fact]
    public async Task GetAsync_WhenParkingNotFound_ReturnsError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetParkingRequest(id);
        _requestValidator.Validate(request).Returns([]);

        // Act
        var result = await _service.GetAsync(request);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
        result.Errors.First().Should().BeEquivalentTo(Errors.Parking.NotFound(request.Id));
    }
    
    private static Domain.Parking CreateDefaultParkingEntity(Guid parkingId,
        DefaultUserSessionData defaultUserSessionData)
    {
        var parking = new Domain.Parking
        {
            Id = parkingId,
            ProviderId = defaultUserSessionData.UserId,
            Name = "Sample Parking Lot",
            Description = "A description of the parking lot",
            Address = new Address("CountryName", "CityName", "StreetName", "123"),
            Latitude = 40.7128,
            Longitude = -74.0060,
            TotalSpaces = 100
        };

        return parking;
    }

    private static AddParkingRequest CreateAddParkingRequest() =>
        new("Name", "Description", new Address("Country", "City", "Street", "123"),
            10.11, 10.11, 100);

    private static UpdateParkingRequest CreateUpdateParkingRequest(Guid parkingId) =>
        new(parkingId, "New Name", "New Description",
            new Address("New Country", "New City", "New Street", "456"), 200);
}