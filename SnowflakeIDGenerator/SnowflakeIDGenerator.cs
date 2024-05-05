// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using SnowflakeID.Helpers;
using System;
using System.Threading;

namespace SnowflakeID
{
    /// <summary>
    /// Generator class for <see cref="SnowflakeID"/>.
    /// <para>This keeps track of time, machine number and sequence.</para>
    /// </summary>
    public class SnowflakeIDGenerator
    {
        private readonly ulong MACHINE_ID;

        private static ulong _Sequence;
        private static ulong Sequence { get => _Sequence; set => _Sequence = value % Snowflake.MaxSequence; }

        private static readonly object lockObject = new();

        /// <summary>
        /// Default date used as epoch if not configured
        /// </summary>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static readonly DateTime DefaultEpoch = DateTime.UnixEpoch;
#else
        // This should be DateTime.UnixEpoch. However, that constant is only available in .netCore and net5 or newer
        public static readonly DateTime DefaultEpoch = new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
#endif

        private readonly DateTime configuredEpoch;

        private static void SetLastTimestampDriftCorrected(ulong timestamp, DateTime epoch)
        {
            LastTimestampDriftCorrected = timestamp + DateTimeHelper.TimestampMillisFromEpoch(epoch, DefaultEpoch);
        }

        private ulong LastTimeStamp => LastTimestampDriftCorrected - DateTimeHelper.TimestampMillisFromEpoch(configuredEpoch, DefaultEpoch);
        private static ulong LastTimestampDriftCorrected;

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number using a custom date as epoch
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        [CLSCompliant(false)]
        public SnowflakeIDGenerator(ulong machineId, DateTime customEpoch)
        {
            if (machineId >= Snowflake.MaxMachineId)
            {
                throw new ArgumentOutOfRangeException(nameof(machineId), $"{nameof(machineId)} must be less than {Snowflake.MaxMachineId}. Got: {machineId}.");
            }
            MACHINE_ID = machineId;
            configuredEpoch = customEpoch;

            // to prevent a weird overflow bug, set last timestamp as the previous millisecond when first creating the generator
            if (LastTimestampDriftCorrected == default) { SetLastTimestampDriftCorrected(DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, configuredEpoch) - 1, customEpoch); }
        }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        [CLSCompliant(false)]
        public SnowflakeIDGenerator(ulong machineId) : this(machineId, DefaultEpoch) { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number using a custom date as epoch
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        public SnowflakeIDGenerator(int machineId, DateTime customEpoch) : this((ulong)machineId, customEpoch) { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        public SnowflakeIDGenerator(int machineId) : this(machineId, DefaultEpoch) { }

        /// <summary>
        /// Gets next Snowflake id
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        public Snowflake GetSnowflake()
        {
            lock (lockObject)
            {
                ulong currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, configuredEpoch);

                if (Sequence == 0 && currentTimestampMillis == LastTimeStamp)
                {
                    do
                    {
                        Thread.Sleep(1);
                        currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, configuredEpoch);
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
                SetLastTimestampDriftCorrected(currentTimestampMillis, configuredEpoch);

                Snowflake snowflake = new(configuredEpoch)
                {
                    Timestamp = currentTimestampMillis,
                    MachineId = MACHINE_ID,
                    Sequence = Sequence,
                };

                Sequence++;

                return snowflake;
            }
        }

        /// <summary>
        /// Gets next Snowflake as number (<typeparamref cref="ulong">ulong</typeparamref>)
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="ulong">ulong</typeparam>
        [CLSCompliant(false)]
        public ulong GetCode()
        {
            return GetSnowflake().Id;
        }

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref>
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="string">string</typeparam>
        public string GetCodeString()
        {
            return GetSnowflake().Code;
        }

        /// <summary>
        /// Static method
        /// Gets next Snowflake id for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        /// <param name="machineId">Machine number</param>
        [CLSCompliant(false)]
        public static Snowflake GetSnowflake(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetSnowflake();
        }

        /// <summary>
        /// Static method
        /// Gets next Snowflake id for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref> using a custom date as epoch
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        [CLSCompliant(false)]
        public static Snowflake GetSnowflake(ulong machineId, DateTime customEpoch)
        {
            return new SnowflakeIDGenerator(machineId, customEpoch).GetSnowflake();
        }

        /// <summary>
        /// Static method
        /// Gets next Snowflake id for a given <typeparamref cref="int"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        /// <param name="machineId">Machine number</param>
        public static Snowflake GetSnowflake(int machineId) => GetSnowflake((ulong)machineId);

        /// <summary>
        /// Static method
        /// Gets next Snowflake id for a given <typeparamref cref="int"><paramref name="machineId"/></typeparamref> using a custom date as epoch
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        public static Snowflake GetSnowflake(int machineId, DateTime customEpoch) => GetSnowflake((ulong)machineId, customEpoch);

        /// <summary>
        /// Static method
        /// Gets next Snowflake as <typeparamref cref="ulong">ulong</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <returns></returns>
        /// <param name="machineId">Machine number</param>
        /// <typeparam cref="ulong">ulong</typeparam>
        [CLSCompliant(false)]
        public static ulong GetCode(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetCode();
        }

        /// <summary>
        /// Static method
        /// Gets next Snowflake as <typeparamref cref="ulong">ulong</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref> using a custom date as epoch
        /// </summary>
        /// <returns></returns>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <typeparam cref="ulong">ulong</typeparam>
        [CLSCompliant(false)]
        public static ulong GetCode(ulong machineId, DateTime customEpoch)
        {
            return new SnowflakeIDGenerator(machineId, customEpoch).GetCode();
        }

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string GetCodeString(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetCodeString();
        }

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref> using a custom date as epoch
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string GetCodeString(ulong machineId, DateTime customEpoch)
        {
            return new SnowflakeIDGenerator(machineId, customEpoch).GetCodeString();
        }

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="int"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public static string GetCodeString(int machineId) => GetCodeString((ulong)machineId);

        /// <summary>
        /// Gets next Snowflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="int"><paramref name="machineId"/></typeparamref> using a custom date as epoch
        /// </summary>
        /// <param name="machineId"></param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <returns></returns>
        public static string GetCodeString(int machineId, DateTime customEpoch) => GetCodeString((ulong)machineId, customEpoch);
    }
}
