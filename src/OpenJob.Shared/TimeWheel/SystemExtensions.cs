namespace System;

public static class SystemExtensions
{
    public static long GetTimestamp(this DateTime dateTime, bool isSecond = false)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime);

        return isSecond
            ? dateTimeOffset.ToUnixTimeSeconds()
            : dateTimeOffset.ToUnixTimeMilliseconds();
    }
}
