namespace UserWsHandler.Services;

public interface IWsManagerService
{
    Task SendUserConnectedMessage(string userId);
    Task SendUserDisconnectedMessage(string userId);
}