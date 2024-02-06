namespace User.NotificationService.Common;

public static class RequestPolly
{
    public const int MedianFirstRetryDelaySeconds = 2;
    public const int DefaultRetryCount = 3;
}