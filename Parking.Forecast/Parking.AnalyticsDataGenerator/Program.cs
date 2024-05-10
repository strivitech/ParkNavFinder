using System.Text.Json;

// List of parking IDs, each representing a unique parking area.
var parkingIds = new List<string>
{
    "84ad9d8e-2503-4552-85bd-291dbe43ac53", "7c750bd6-8fec-4e45-989f-079f19b1e0d7",
    "1bfb659e-698c-4d66-accd-88311313cdd5", "b32e831e-e276-4861-8437-16a3f368f411",
    "e0fdd45a-296b-4ed9-bd67-2f4eb3997f56", "96c6102c-1eab-4cd3-b666-31c9a053b91b",
    "be13237d-7792-4c0b-8662-7d1defb01da6", "e8159f74-37ba-426e-b613-c13c3dd1622b",
    "37dbd008-7fca-4998-9d7c-cef5313df90a"
};

// Initialize parking spaces for each ID with a random value between 50 and 500.
var totalParkingSpaces = InitializeParkingSpaces(parkingIds);
var startTime = DateTime.UtcNow.AddMonths(-2); // Sets the starting time to two months ago.
int intervalsToGenerate = 8640; // Total intervals for 30 days at 5-minute intervals.

string basePath = @"E:\Projects\LocalData\ParkNavFinder\ParkingStatesData";
string parkingDataPath = @"E:\Projects\LocalData\ParkNavFinder\ParkingData";
Directory.CreateDirectory(basePath); // Ensures the directory exists for storing data files.
Directory.CreateDirectory(parkingDataPath); // Ensures the directory exists for storing parking data.

SaveParkingSpaces(totalParkingSpaces, parkingDataPath);
GenerateData(startTime, intervalsToGenerate, basePath, parkingIds, totalParkingSpaces);

// Initializes parking spaces with a random number of spaces for each parking ID.
Dictionary<string, int> InitializeParkingSpaces(List<string> ids)
{
    var spaces = new Dictionary<string, int>();
    foreach (var id in ids)
    {
        spaces[id] = Random.Shared.Next(50, 501); // Randomly assigns parking spaces between 50 and 500.
    }
    return spaces;
}

// Generates parking data over the specified time intervals.
void GenerateData(DateTime start, int intervals, string path, List<string> ids, Dictionary<string, int> spaces)
{
    for (int i = 0; i < intervals; i++)
    {
        var data = new List<object>();
        DateTime currentTime = start.AddMinutes(5 * i);
        foreach (var id in ids)
        {
            int observers = CalculateObservers(id, currentTime, spaces);
            double probability = CalculateProbability(id, observers, spaces);
            data.Add(CreateDataEntry(id, observers, probability, currentTime));
        }
        SaveDataToFile(data, path);
    }
}

// Calculates the number of observers based on time of day and parking capacity.
int CalculateObservers(string parkingId, DateTime currentTime, Dictionary<string, int> spaces)
{
    int baseObserverCount = Random.Shared.Next(1, spaces[parkingId] + 1);
    int peakFactor = Random.Shared.Next(100);
    bool isPeakHour = (currentTime.Hour is >= 7 and <= 9 || currentTime.Hour is >= 16 and <= 18) && peakFactor < 75; // 75% chance of being peak hour.
    bool isLowActivity = currentTime.Hour is >= 23 or < 7; // Identifies low activity hours.

    if (isLowActivity)
    {
        return Random.Shared.Next(1, Math.Max(2, (int)(spaces[parkingId] * 0.1) + 1));
    }
    if (isPeakHour || peakFactor < 15) // Adds a peak condition during normal hours.
    {
        return Random.Shared.Next(spaces[parkingId], spaces[parkingId] * 2 + 1);
    }
    return baseObserverCount;
}

// Calculates the probability of a parking space being observed based on the number of observers.
double CalculateProbability(string parkingId, int observers, Dictionary<string, int> spaces)
{
    double probability = (double)spaces[parkingId] / observers;
    return probability > 1 ? 1 : probability; // Ensures the probability does not exceed 1.
}

// Creates a data entry for each parking interval.
object CreateDataEntry(string parkingId, int observers, double probability, DateTime calculatedAtUtc)
{
    return new
    {
        ParkingId = parkingId,
        TotalObservers = observers,
        Probability = probability,
        CalculatedAtUtc = calculatedAtUtc.ToString("o") // Formats the date in ISO 8601 format.
    };
}

// Saves generated data to a JSON file in the specified path.
void SaveDataToFile(List<object> data, string path)
{
    string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    string fileName = Guid.NewGuid() + ".json"; // Generates a unique file name for each data set.
    string fullPath = Path.Combine(path, fileName);
    File.WriteAllText(fullPath, json); // Writes data to the file.
}

// Saves the initial parking spaces data to a JSON file.
void SaveParkingSpaces(Dictionary<string, int> spaces, string path)
{
    string json = JsonSerializer.Serialize(spaces, new JsonSerializerOptions { WriteIndented = true });
    string filePath = Path.Combine(path, "ParkingSpaces.json");
    File.WriteAllText(filePath, json); // Writes parking spaces data to the file.
}