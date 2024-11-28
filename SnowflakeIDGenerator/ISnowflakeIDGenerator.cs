// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID
{
    /// <summary>
    /// Interface for the generator class for <see cref="Snowflake"/>.
    /// <para>This keeps track of time, machine number, and sequence.</para>
    /// </summary>
    /// <remarks>
    /// <para><see href="https://www.nuget.org/packages/SnowflakeIDGenerator">NuGet</see></para>
    /// <para><seealso href="https://github.com/fenase/SnowflakeIDGenerator">Source</seealso></para>
    /// <para><seealso href="https://fenase.github.io/SnowflakeIDGenerator/api/SnowflakeID.html">API</seealso></para>
    /// <para><seealso href="https://fenase.github.io/projects/SnowflakeIDGenerator">Site</seealso></para>
    /// </remarks>
    [CLSCompliant(false)]
    public interface ISnowflakeIDGenerator
    {
        /// <summary>
        /// Gets the date configured as the epoch for the generator.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime"/> value representing the custom epoch date.
        /// </value>
        /// <remarks>
        /// The epoch date is used as the starting point for generating unique IDs.
        /// </remarks>
        DateTime ConfiguredEpoch { get; }

        /// <summary>
        /// Gets the configured machine ID for the generator.
        /// </summary>
        /// <value>
        /// The <see cref="int"/> value representing the machine ID.
        /// </value>
        /// <remarks>
        /// The machine ID is used to ensure uniqueness across different instances of the generator.
        /// </remarks>
        int ConfiguredMachineId { get; }

        /// <summary>
        /// Gets the next Snowflake ID as a number.
        /// </summary>
        /// <returns>A <see cref="ulong"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// This method generates a new Snowflake ID and returns it as a numeric value.
        /// </remarks>
        ulong GetCode();

        /// <summary>
        /// Gets the next Snowflake ID as a string.
        /// </summary>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// This method generates a new Snowflake ID and returns it as a string value.
        /// </remarks>
        string GetCodeString();

        /// <summary>
        /// Generates the next Snowflake ID.
        /// </summary>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        /// <remarks>
        /// This method generates a new Snowflake ID and returns it as a <see cref="Snowflake"/> object.
        /// </remarks>
        Snowflake GetSnowflake();
    }
}
