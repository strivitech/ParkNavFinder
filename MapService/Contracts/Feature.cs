namespace MapService.Contracts;

public class Feature
{
    public Geometry Geometry { get; set; } = null!;
    public Properties Properties { get; set; } = null!;
}