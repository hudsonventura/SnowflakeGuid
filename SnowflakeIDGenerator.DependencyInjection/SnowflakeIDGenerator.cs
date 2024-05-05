// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using SIdG = SnowflakeID;

namespace SnowflakeID.DependencyInjection
{
    /// <summary>
    /// Generator class for <see cref="SnowflakeID"/>.
    /// <para>This keeps track of time, machine number and sequence.</para>
    /// </summary>
    public class SnowflakeIDGenerator(IOptions<SnowflakeIdGeneratorOptions> snkOptions) : SIdG.SnowflakeIDGenerator((ulong)snkOptions.Value.MachineId, snkOptions.Value.Epoch), ISnowflakeIDGenerator
    {
    }
}
