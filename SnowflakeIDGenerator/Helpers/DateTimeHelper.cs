using System;

namespace SnowflakeID.Helpers
{
    internal static class DateTimeHelper
    {
        internal static ulong TimestampMillisFromEpoch(DateTime date, DateTime epoch)
        {
            return (ulong)date.Subtract(epoch).Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
