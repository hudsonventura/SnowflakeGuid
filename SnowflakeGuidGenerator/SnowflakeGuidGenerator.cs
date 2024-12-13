﻿// Copyright (c) 2022-2025, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using SnowflakeID.Helpers;
using System;
using System.Threading;

namespace SnowflakeID
{
    /// <summary>
    /// Generator class for <see cref="Snowflake"/>.
    /// <para>This keeps track of time, machine number and sequence.</para>
    /// </summary>
    /// <remarks>
    /// <para><see href="https://www.nuget.org/packages/SnowflakeIDGenerator">NuGet</see></para>
    /// <para><seealso href="https://github.com/fenase/SnowflakeIDGenerator">Source</seealso></para>
    /// <para><seealso href="https://fenase.github.io/SnowflakeIDGenerator/api/SnowflakeID.html">API</seealso></para>
    /// <para><seealso href="https://fenase.github.io/projects/SnowflakeIDGenerator">Site</seealso></para>
    /// </remarks>
    public class SnowflakeGuidGenerator : ISnowflakeIDGenerator, ISnowflakeIDGeneratorClsCompliant
    {
        private readonly ulong MACHINE_ID;

        private static ulong _Sequence;
        private static ulong Sequence { get => _Sequence; set => _Sequence = value % Snowflake.MaxSequence; }

#if NET9_0_OR_GREATER
        private static readonly Lock lockObject = new();
#else
        private static readonly object lockObject = new();
#endif

        /// <summary>
        /// Gets the date configured as the epoch for the generator.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime"/> value representing the custom epoch date.
        /// </value>
        /// <remarks>
        /// The epoch date is used as the starting point for generating unique IDs.
        /// </remarks>
        public DateTime ConfiguredEpoch { get; }

        /// <summary>
        /// Gets the configured machine ID for the generator.
        /// </summary>
        /// <value>
        /// The <see cref="int"/> value representing the machine ID.
        /// </value>
        /// <remarks>
        /// The machine ID is used to ensure uniqueness across different instances of the generator.
        /// </remarks>
        public int ConfiguredMachineId => (int)MACHINE_ID;

        private static void SetLastTimestampDriftCorrected(ulong timestamp, DateTime epoch) =>
            LastTimestampDriftCorrected = timestamp + DateTimeHelper.TimestampMillisFromEpoch(epoch, GlobalConstants.DefaultEpoch);

        private ulong LastTimeStamp => LastTimestampDriftCorrected - DateTimeHelper.TimestampMillisFromEpoch(ConfiguredEpoch, GlobalConstants.DefaultEpoch);
        private static ulong LastTimestampDriftCorrected;

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number using a custom date as epoch.
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="machineId"/> is greater than or equal to <see cref="Snowflake.MaxMachineId"/>.
        /// </exception>
        /// <remarks>
        /// This constructor initializes the generator with a specific machine ID and a custom epoch date.
        /// It ensures that the machine ID is within the valid range and sets the initial timestamp to prevent overflow issues.
        /// </remarks>
        [CLSCompliant(false)]
        public SnowflakeGuidGenerator(ulong machineId, DateTime customEpoch)
        {
            if (machineId >= Snowflake.MaxMachineId)
            {
                throw new ArgumentOutOfRangeException(nameof(machineId), $"{nameof(machineId)} must be less than {Snowflake.MaxMachineId}. Got: {machineId}.");
            }
            MACHINE_ID = machineId;
            ConfiguredEpoch = customEpoch;

            // to prevent a weird overflow bug, set last timestamp as the previous millisecond when first creating the generator
            if (LastTimestampDriftCorrected == default) { SetLastTimestampDriftCorrected(DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, ConfiguredEpoch) - 1, customEpoch); }
        }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number.
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="machineId"/> is greater than or equal to <see cref="Snowflake.MaxMachineId"/>.
        /// </exception>
        /// <remarks>
        /// This constructor initializes the generator with a specific machine ID and uses the default epoch date.
        /// It ensures that the machine ID is within the valid range and sets the initial timestamp to prevent overflow issues.
        /// </remarks>
        [CLSCompliant(false)]
        public SnowflakeGuidGenerator(ulong machineId) : this(machineId, GlobalConstants.DefaultEpoch) { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number using a custom date as epoch.
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="machineId"/> is greater than or equal to <see cref="Snowflake.MaxMachineId"/>.
        /// </exception>
        /// <remarks>
        /// This constructor initializes the generator with a specific machine ID and a custom epoch date.
        /// It ensures that the machine ID is within the valid range and sets the initial timestamp to prevent overflow issues.
        /// </remarks>
        public SnowflakeGuidGenerator(int machineId, DateTime customEpoch) : this((ulong)machineId, customEpoch) { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number.
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="machineId"/> is greater than or equal to <see cref="Snowflake.MaxMachineId"/>.
        /// </exception>
        /// <remarks>
        /// This constructor initializes the generator with a specific machine ID and uses the default epoch date.
        /// It ensures that the machine ID is within the valid range and sets the initial timestamp to prevent overflow issues.
        /// </remarks>
        public SnowflakeGuidGenerator(int machineId) : this(machineId, GlobalConstants.DefaultEpoch) { }



        public Guid GetGuid(){
            return GetSnowflake().Guid;
        }


        /// <summary>
        /// Generates the next Snowflake ID.
        /// </summary>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the system clock is moved backwards.
        /// </exception>
        public Snowflake GetSnowflake()
        {
            lock (lockObject)
            {
                ulong currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, ConfiguredEpoch);

                if (Sequence == 0 && currentTimestampMillis == LastTimeStamp)
                {
                    do
                    {
                        Thread.Sleep(1);
                        currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, ConfiguredEpoch);
                    } while (currentTimestampMillis == LastTimeStamp);
                }
                else if (currentTimestampMillis < LastTimeStamp)
                {
                    throw new InvalidOperationException("Time moved backwards!");
                }
                else if (currentTimestampMillis > LastTimeStamp)
                {
                    Sequence = 0;
                }
                SetLastTimestampDriftCorrected(currentTimestampMillis, ConfiguredEpoch);

                Snowflake snowflake = new(ConfiguredEpoch)
                {
                    Timestamp = currentTimestampMillis,
                    MachineId = MACHINE_ID,
                    Sequence = Sequence,
                };

                snowflake.Guid = snowflake.ToGuid();

                Sequence++;

                return snowflake;
            }
        }

        /// <summary>
        /// Gets the next Snowflake ID as a number.
        /// </summary>
        /// <returns>A <see cref="ulong"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// <para>This method generates a new Snowflake ID and returns it as a numeric value.</para>
        /// <para>If a loosely typed language (or a language that doesn't differentiate between number types, i.e.: Typescript)
        /// is part of your workflow, use <see cref="GetCodeString()"/> to avoid issues regarding
        /// floating-point underflow and rounding.</para>
        /// </remarks>
        [CLSCompliant(false)]
        public ulong GetCode() => GetSnowflake().Id;

        /// <summary>
        /// Gets the next Snowflake ID as a string.
        /// </summary>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// This method generates a new Snowflake ID and returns it as a string value.
        /// </remarks>
        public string GetCodeString() => GetSnowflake().Code;

        /// <summary>
        /// Static method to get the next Snowflake ID for a given machine ID.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        /// <remarks>
        /// This method generates a new Snowflake ID and returns it as a <see cref="Snowflake"/> object.
        /// </remarks>
        [CLSCompliant(false)]
        public static Snowflake GetSnowflake(ulong machineId) => new SnowflakeGuidGenerator(machineId).GetSnowflake();

        /// <summary>
        /// Static method to get the next Snowflake ID for a given machine ID using a custom epoch date.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <param name="customEpoch">The custom epoch date as a <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        [CLSCompliant(false)]
        public static Snowflake GetSnowflake(ulong machineId, DateTime customEpoch) =>
            new SnowflakeGuidGenerator(machineId, customEpoch).GetSnowflake();

        /// <summary>
        /// Static method to get the next Snowflake ID for a given machine ID.
        /// </summary>
        /// <param name="machineId">The machine ID as an <see cref="int"/>.</param>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        public static Snowflake GetSnowflake(int machineId = 0) => GetSnowflake((ulong)machineId);
        

        /// <summary>
        /// Static method to get the next Snowflake ID for a given machine ID using a custom epoch date.
        /// </summary>
        /// <param name="machineId">The machine ID as an <see cref="int"/>.</param>
        /// <param name="customEpoch">The custom epoch date as a <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
        public static Snowflake GetSnowflake(int machineId, DateTime customEpoch) => GetSnowflake((ulong)machineId, customEpoch);

        /// <summary>
        /// Static method to get the next Snowflake ID as a number for a given machine ID.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <returns>A <see cref="ulong"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// <para>This method generates a new Snowflake ID and returns it as a numeric value.</para>
        /// <para>If a loosely typed language (or a language that doesn't differentiate between number types, i.e.: Typescript)
        /// is part of your workflow, use <see cref="GetCodeString(ulong)"/> to avoid issues regarding
        /// floating-point underflow and rounding.</para>
        /// </remarks>
        [CLSCompliant(false)]
        public static ulong GetCode(ulong machineId) => new SnowflakeGuidGenerator(machineId).GetCode();

        /// <summary>
        /// Static method to get the next Snowflake ID as a number for a given machine ID using a custom epoch date.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <param name="customEpoch">The custom epoch date as a <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="ulong"/> representing the next Snowflake ID.</returns>
        /// <remarks>
        /// <para>This method generates a new Snowflake ID and returns it as a numeric value.</para>
        /// <para>If a loosely typed language (or a language that doesn't differentiate between number types, i.e.: Typescript)
        /// is part of your workflow, use <see cref="GetCodeString(ulong, DateTime)"/> to avoid issues regarding
        /// floating-point underflow and rounding.</para>
        /// </remarks>
        [CLSCompliant(false)]
        public static ulong GetCode(ulong machineId, DateTime customEpoch) => new SnowflakeGuidGenerator(machineId, customEpoch).GetCode();

        /// <summary>
        /// Gets the next Snowflake ID as a string for a given machine ID.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        [CLSCompliant(false)]
        public static string GetCodeString(ulong machineId) => new SnowflakeGuidGenerator(machineId).GetCodeString();

        /// <summary>
        /// Gets the next Snowflake ID as a string for a given machine ID using a custom epoch date.
        /// </summary>
        /// <param name="machineId">The machine ID as a <see cref="ulong"/>.</param>
        /// <param name="customEpoch">The custom epoch date as a <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        [CLSCompliant(false)]
        public static string GetCodeString(ulong machineId, DateTime customEpoch) => new SnowflakeGuidGenerator(machineId, customEpoch).GetCodeString();

        /// <summary>
        /// Gets the next Snowflake ID as a string for a given machine ID.
        /// </summary>
        /// <param name="machineId">The machine ID as an <see cref="int"/>.</param>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        public static string GetCodeString(int machineId) => GetCodeString((ulong)machineId);

        /// <summary>
        /// Gets the next Snowflake ID as a string for a given machine ID using a custom epoch date.
        /// </summary>
        /// <param name="machineId">The machine ID as an <see cref="int"/>.</param>
        /// <param name="customEpoch">The custom epoch date as a <see cref="DateTime"/>.</param>
        /// <returns>A <see cref="string"/> representing the next Snowflake ID.</returns>
        public static string GetCodeString(int machineId, DateTime customEpoch) => GetCodeString((ulong)machineId, customEpoch);





        
    }
}
