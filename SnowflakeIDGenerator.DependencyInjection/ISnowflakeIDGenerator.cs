// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

namespace SnowflakeID.DependencyInjection
{
    /// <summary>
    /// Generator class for <see cref="SnowflakeID"/>.
    /// <para>This keeps track of time, machine number and sequence.</para>
    /// </summary>
    [System.CLSCompliant(false)]
    public interface ISnowflakeIDGenerator
    {
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
