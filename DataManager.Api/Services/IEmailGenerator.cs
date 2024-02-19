namespace DataManager.Api.Services;

public interface IEmailGenerator
{
    public string Generate(string startsWith);
}