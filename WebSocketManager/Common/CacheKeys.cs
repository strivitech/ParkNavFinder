namespace WebSocketManager.Common;

public static class CacheKeys
{
    public const string UserPrefix = "user-";
    
    public static string UserKey(string userId) => $"{UserPrefix}{userId}";
}