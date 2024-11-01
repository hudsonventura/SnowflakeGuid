// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using SnowflakeID.Helpers;
using System;
using System.Globalization;

namespace SnowflakeID
{
    /// <summary>
    /// Option object for <see cref="SnowflakeIDGenerator"/>.
    /// </summary>
    public class SnowflakeIdGeneratorOptions
    {
        /// <summary>
        /// Gets or sets the machine ID.
        /// </summary>
        /// <value>The machine ID as an <see cref="int"/>.</value>
        public int MachineId { get; set; }

        /// <summary>
        /// Gets or sets the epoch date as a <see cref="DateTime"/> object.
        /// </summary>
        /// <value>The epoch date as a <see cref="DateTime"/> object.</value>
        internal DateTime EpochObject { get; set; } = GlobalConstants.DefaultEpoch;

        /// <summary>
        /// Gets or sets the epoch date as a string.
        /// </summary>
        /// <value>The epoch date as a string in ISO 8601 format.</value>
        /// <remarks>
        /// This property allows setting the epoch date using a string representation.
        /// The date is parsed and stored internally as a <see cref="DateTime"/> object in UTC.
        /// </remarks>
        public string Epoch
        {
            get => EpochObject.ToString("O", CultureInfo.InvariantCulture);
            set => EpochObject = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite).ToUniversalTime();
        }
    }
}
