using Bogus;
using DataManager.Api.Contracts;

namespace DataManager.Api.Services;

public class ParkingGenerator(IConfiguration configuration) : IParkingGenerator
{
    private readonly IConfiguration _configuration = configuration;

    public IEnumerable<CreateParkingRequest> GenerateRealisticParkingData(int count)
    {
        var faker = new Faker();
        for (int i = 0; i < count; i++)
        {
            var address = faker.Address;
            yield return new CreateParkingRequest(
                Name: faker.Company.CompanyName(),
                Description: faker.Lorem.Paragraph(),
                Address: new Address(
                    Country: address.Country(),
                    City: address.City(),
                    Street: address.StreetName(),
                    StreetNumber: address.BuildingNumber()),
                Latitude: address.Latitude(_configuration.GetValue<double>("Generator:Parking:LatitudeMin"),
                    _configuration.GetValue<double>("Generator:Parking:LatitudeMax")),
                Longitude: address.Longitude(_configuration.GetValue<double>("Generator:Parking:LongitudeMin"),
                    _configuration.GetValue<double>("Generator:Parking:LongitudeMax")),
                TotalSpaces: faker.Random.Int(_configuration.GetValue<int>("Generator:Parking:TotalSpacesMin"),
                    _configuration.GetValue<int>("Generator:Parking:TotalSpacesMax")));
        }
    }
}