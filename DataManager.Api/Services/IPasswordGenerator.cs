namespace DataManager.Api.Services;

public interface IPasswordGenerator
{
    public string Generate(int length);
}