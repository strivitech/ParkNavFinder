using H3;
using MapService.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MapService.Tests;

public class HexagonControllerTests
{
    private readonly HexagonController _controller = new();

    [Fact]
    public void GetH3Index_ReturnsOkResult_WithValidCoordinates()
    {
        // Arrange
        double lat = 40.7128;
        double lon = -74.0060;
        int resolution = 7;

        // Act
        var result = _controller.GetH3Index(lat, lon, resolution);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<string>(okResult.Value);
    }
    
    [Theory]
    [InlineData(-91, 0, 9)] // Extremely low latitude
    [InlineData(91, 0, 9)] // Extremely high latitude
    [InlineData(0, -181, 9)] // Extremely low longitude
    [InlineData(0, 181, 9)] // Extremely high longitude
    [InlineData(40.7128, -74.0060, -1)] // Invalid resolution
    public void GetH3Index_ReturnsBadRequestOrServerError_ForInvalidInput(double lat, double lon, int resolution)
    {
        // Act
        var result = _controller.GetH3Index(lat, lon, resolution);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}