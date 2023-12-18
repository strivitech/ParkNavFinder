namespace UserService.Common;

internal static class CacheKeys
{
    public static string UserKey(string userId) => $"user-{userId}";
}