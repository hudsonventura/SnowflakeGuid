// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID
{
    /// <summary>
    /// Generator class for <see cref="Snowflake"/>.
    /// <para>This keeps track of time, machine number and sequence.</para>
    /// </summary>
    [CLSCompliant(false)]
    public interface ISnowflakeIDGenerator
    {
        /// <summary>
        /// Date configured as epoch for the generator
        /// </summary>
        DateTime ConfiguredEpoch { get; }

        /// <summary>
        /// Configured instance id for the generator
        /// </summary>
        int ConfiguredMachineId { get; }

        /// <summary>
        /// Gets next Snowflake as number (<typeparamref cref="ulong">ulong</typeparamref>)
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="ulong">ulong</typeparam>
        ulong GetCode();

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref>
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="string">string</typeparam>
        string GetCodeString();

        /// <summary>
        /// Gets next Snowflake id
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        Snowflake GetSnowflake();
    }
}
