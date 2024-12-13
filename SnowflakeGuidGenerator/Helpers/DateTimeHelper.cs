// Copyright (c) 2022-2025, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID.Helpers
{
    /// <summary>
    /// Helper class for DateTime operations related to Snowflake ID generation.
    /// </summary>
    internal static class DateTimeHelper
    {
        /// <summary>
        /// Calculates the timestamp in milliseconds from a given date relative to a specified epoch.
        /// </summary>
        /// <param name="date">The date for which to calculate the timestamp.</param>
        /// <param name="epoch">The epoch date to use as the reference point.</param>
        /// <returns>The timestamp in milliseconds as an <see cref="ulong"/>.</returns>
        internal static ulong TimestampMillisFromEpoch(DateTime date, DateTime epoch) =>
            (ulong)date.Subtract(epoch).Ticks / TimeSpan.TicksPerMillisecond;
    }
}
