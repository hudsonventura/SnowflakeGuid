// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using SnowflakeID.Helpers;
using System;
using System.Globalization;

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
        public ulong MachineId { get; set; } = 1;

        internal DateTime EpochObject { get; set; } = GlobalConstants.DefaultEpoch;

        /// <summary>
        /// String representing date to use as epoch
        /// Use this in options
        /// </summary>
        public string Epoch
        {
            get => EpochObject.ToString("O", CultureInfo.InvariantCulture);
            set => EpochObject = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite).ToUniversalTime();
        }
    }
}
