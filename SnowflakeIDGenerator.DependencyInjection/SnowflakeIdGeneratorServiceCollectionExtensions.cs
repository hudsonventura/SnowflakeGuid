// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SnowflakeID.Helpers;
using System;

namespace SnowflakeID
{
    /// <summary>
    ///Extension method to register  <see cref="SnowflakeIDGenerator"/>
    /// </summary>
    public static class SnowflakeIdGeneratorServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(
        this IServiceCollection serviceCollection) => serviceCollection.AddSnowflakeIdGeneratorService(new SnowflakeIdGeneratorOptions());

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(
        this IServiceCollection serviceCollection, ulong machineId, DateTime customEpoch)
        {
            serviceCollection.TryAddSingleton<ISnowflakeIDGenerator>(sp => new SnowflakeIDGenerator(machineId, customEpoch));

            return serviceCollection;
        }

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(this IServiceCollection serviceCollection, long machineId, DateTime customEpoch) => serviceCollection.AddSnowflakeIdGeneratorService((ulong)machineId, customEpoch);

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="machineId">Machine number</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(this IServiceCollection serviceCollection, ulong machineId) => serviceCollection.AddSnowflakeIdGeneratorService(machineId, GlobalConstants.DefaultEpoch);

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(this IServiceCollection serviceCollection, int machineId, DateTime customEpoch) => serviceCollection.AddSnowflakeIdGeneratorService((ulong)machineId, customEpoch);

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="machineId">Machine number</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(this IServiceCollection serviceCollection, int machineId) => serviceCollection.AddSnowflakeIdGeneratorService(machineId, GlobalConstants.DefaultEpoch);

        /// <summary>
        /// Registers a <see cref="SnowflakeIDGenerator"/> in <see cref="ISnowflakeIDGenerator"/>
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="options">Option object. Useful when obtaining from IConfigurationSection</param>
        /// <returns></returns>
        public static IServiceCollection AddSnowflakeIdGeneratorService(
        this IServiceCollection serviceCollection, SnowflakeIdGeneratorOptions options)
        {
            options ??= new SnowflakeIdGeneratorOptions();
            return serviceCollection.AddSnowflakeIdGeneratorService(options.MachineId, options.EpochObject);
        }
    }
}
