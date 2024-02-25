namespace User.NotificationService.Common;

public static class RequestPolly
{
    public const int MedianFirstRetryDelaySeconds = 1;
    public const int DefaultRetryCount = 2;
}