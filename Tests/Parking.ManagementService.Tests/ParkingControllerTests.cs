using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Parking.ManagementService.Contracts;
using Parking.ManagementService.Controllers;
using Parking.ManagementService.Domain;
using Parking.ManagementService.Services;

namespace Parking.ManagementService.Tests;

public class ParkingControllerTests
{
    private readonly IParkingService _parkingService;
    private readonly ParkingController _controller;

    public ParkingControllerTests()
    {
        _parkingService = Substitute.For<IParkingService>();
        _controller = new ParkingController(_parkingService);
    }

    [Fact]
    public async Task Get_WhenSuccessful_ReturnsOkObjectResultWithParkingDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedResponse = new GetParkingResponse(
            id, "ProviderId", "ParkingName", "Description", 
            new Address("Country", "City", "Street", "123"),
            10.11, 10.11, 100);
        _parkingService.GetAsync(Arg.Is<GetParkingRequest>(x => x.Id == id)).Returns(expectedResponse);

        // Act
        var result = await _controller.Get(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Get_WhenValidationFails_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var validationError = Error.Validation("ValidationFailed", "Validation error occurred");
        _parkingService.GetAsync(Arg.Is<GetParkingRequest>(x => x.Id == id)).Returns(validationError);

        // Act
        var result = await _controller.Get(id);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result.Result!;
        badRequestResult.Value.Should().BeEquivalentTo(new { validationError.Code, validationError.Description });
    }
    
    [Fact]
    public async Task Get_WhenNotFound_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var notFoundError = Error.NotFound("NotFound", "Parking not found");
        _parkingService.GetAsync(Arg.Is<GetParkingRequest>(x => x.Id == id)).Returns(notFoundError);

        // Act
        var result = await _controller.Get(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().BeEquivalentTo(new { notFoundError.Code, notFoundError.Description });
    } 
    
      [Fact]
    public async Task Add_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var request = new AddParkingRequest("Name", "Description", new Address("Country", "City", "Street", "123"), 10.11, 10.11, 100);
        _parkingService.AddAsync(request).Returns(new Created());

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Add_WhenValidationFails_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var request = new AddParkingRequest("Name", "Description", new Address("Country", "City", "Street", "123"), 10.11, 10.11, 100);
        var validationError = Error.Validation("ValidationFailed", "Validation error occurred");
        _parkingService.AddAsync(request).Returns(validationError);

        // Act
        var result = await _controller.Add(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result!;
        badRequestResult.Value.Should().BeEquivalentTo(new { validationError.Code, validationError.Description });
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var request = new UpdateParkingRequest(Guid.NewGuid(), "Name", "Description", new Address("Country", "City", "Street", "123"), 100);
        _parkingService.UpdateAsync(request).Returns(new Updated());

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_WhenValidationFails_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var request = new UpdateParkingRequest(Guid.NewGuid(), "Name", "Description", new Address("Country", "City", "Street", "123"), 100);
        var validationError = Error.Validation("ValidationFailed", "Validation error occurred");
        _parkingService.UpdateAsync(request).Returns(validationError);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result!;
        badRequestResult.Value.Should().BeEquivalentTo(new { validationError.Code, validationError.Description });
    }
    
    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var request = new UpdateParkingRequest(Guid.NewGuid(), "Name", "Description", new Address("Country", "City", "Street", "123"), 100);
        var notFoundError = Error.NotFound("NotFound", "Parking not found");
        _parkingService.UpdateAsync(request).Returns(notFoundError);

        // Act
        var result = await _controller.Update(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result!;
        notFoundResult.Value.Should().BeEquivalentTo(new { notFoundError.Code, notFoundError.Description });
    }

    [Fact]
    public async Task Delete_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var request = new DeleteParkingRequest(Guid.NewGuid());
        _parkingService.DeleteAsync(request).Returns(new Deleted());

        // Act
        var result = await _controller.Delete(request);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_WhenValidationFails_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var request = new DeleteParkingRequest(Guid.NewGuid());
        var validationError = Error.Validation("ValidationFailed", "Validation error occurred");
        _parkingService.DeleteAsync(request).Returns(validationError);

        // Act
        var result = await _controller.Delete(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result!;
        badRequestResult.Value.Should().BeEquivalentTo(new { validationError.Code, validationError.Description });
    }
    
    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var request = new DeleteParkingRequest(Guid.NewGuid());
        var notFoundError = Error.NotFound("NotFound", "Parking not found");
        _parkingService.DeleteAsync(request).Returns(notFoundError);

        // Act
        var result = await _controller.Delete(request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result!;
        notFoundResult.Value.Should().BeEquivalentTo(new { notFoundError.Code, notFoundError.Description });
    }
}