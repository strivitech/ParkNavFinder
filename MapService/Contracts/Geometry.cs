namespace MapService.Contracts;

public class Geometry
{
    public List<List<double>> Coordinates { get; set; } = null!; // longitude, latitude
    public string Type { get; set; } = "LineString";
}