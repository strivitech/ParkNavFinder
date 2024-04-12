using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;

namespace Parking.AnalyticsService.Services;

public class ParkingAnalyticsArchivalService(IAmazonS3 s3Client, IConfiguration configuration)
    : IParkingAnalyticsArchivalService
{
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly string _bucketName = configuration["AWS:BucketName"]!;

    public async Task ArchiveAsync<T>(T data)
    {
        var jsonData = JsonSerializer.Serialize(data);
        var key = $"{Guid.NewGuid()}";
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            ContentBody = jsonData,
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(putRequest);
    }
}