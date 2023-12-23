namespace UserWsHandler.Common;

public static class RequestPolly
{
    public const int MedianFirstRetryDelaySeconds = 3;
    public const int DefaultRetryCount = 3;
}