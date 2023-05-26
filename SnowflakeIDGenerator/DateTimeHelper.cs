using System;

namespace SnowflakeID
{
    internal static class DateTimeHelper
    {
        internal static ulong TimestampMillisFromEpoch(DateTime date, DateTime epoch)
        {
            return ((ulong)date.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
        }
    }
}
