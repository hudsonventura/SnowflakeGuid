// Copyright (c) 2022-2025, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace Helpers;

/// <summary>
/// Global constants used throughout the Snowflake ID generator.
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// The default date used as the epoch if not configured.
    /// </summary>
    /// <remarks>
    /// This is set to Unix Epoch (January 1, 1970).
    /// </remarks>
    public static readonly DateTime DefaultEpoch =
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        DateTime.UnixEpoch;
#else
        new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
#endif
}

