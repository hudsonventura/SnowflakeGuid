// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID.Helpers
{
    /// <summary>
    /// Global constants.
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// Default date used as epoch if not configured
        /// </summary>
        public static readonly DateTime DefaultEpoch =
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            DateTime.UnixEpoch;
#else
            new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
#endif
    }
}
