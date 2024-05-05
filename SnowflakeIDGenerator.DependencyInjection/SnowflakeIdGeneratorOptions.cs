// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID.DependencyInjection
{
    /// <summary>
    /// Option object for <see cref="SnowflakeIDGenerator"/>
    /// </summary>
    public class SnowflakeIdGeneratorOptions
    {
        /// <summary>
        /// Machine number
        /// </summary>
        public long MachineId { get; set; } = 1;

        /// <summary>
        /// Date to use as epoch
        /// </summary>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public DateTime Epoch { get; set; } = DateTime.UnixEpoch;
#else
        // This should be DateTime.UnixEpoch. However, that constant is only available in .netCore and net5 or newer
        public DateTime Epoch { get; set; } = new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
#endif
    }
}
