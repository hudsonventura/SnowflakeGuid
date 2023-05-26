// Copyright (c) Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;

[assembly: CLSCompliant(true)]
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
        private static readonly DateTime defaultEpoch = new(1970, 1, 1);
        private readonly DateTime epoch;

        private static ulong LastTimeStamp { get; set; }




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
            epoch = customEpoch;
        }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        [CLSCompliant(false)]
        public SnowflakeIDGenerator(ulong machineId) : this(machineId, defaultEpoch) { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number using a custom date as epoch
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <param name="customEpoch">Date to use as epoch</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        public SnowflakeIDGenerator(int machineId, DateTime customEpoch) : this((ulong)machineId, customEpoch)
        { }

        /// <summary>
        /// Creates a SnowflakeIDGenerator for a given machine number
        /// </summary>
        /// <param name="machineId">Machine number</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="machineId"/> must be less than Snowflake.MaxMachineId</exception>
        public SnowflakeIDGenerator(int machineId) : this(machineId, defaultEpoch)
        { }

        /// <summary>
        /// Gets next Snowflake id
        /// </summary>
        /// <returns><typeparamref cref="Snowflake">Snowflake</typeparamref></returns>
        /// <typeparam cref="Snowflake"/>
        public Snowflake GetSnowflake()
        {
            lock (lockObject)
            {
                ulong currentTimestampMillis = ((ulong)DateTime.UtcNow.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);

                if (Sequence == 0 && currentTimestampMillis == LastTimeStamp)
                {
                    do
                    {
                        Thread.Sleep(1);
                        currentTimestampMillis = ((ulong)DateTime.UtcNow.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
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
                LastTimeStamp = currentTimestampMillis;

                Snowflake snowflake = new()
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
        /// Gets next Snowlflake as number (<typeparamref cref="ulong">ulong</typeparamref>)
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="ulong">ulong</typeparam>
        [CLSCompliant(false)]
        public ulong GetCode()
        {
            return GetSnowflake().Id;
        }

        /// <summary>
        /// Gets next Snowlflake as <typeparamref cref="string">string</typeparamref>
        /// </summary>
        /// <returns></returns>
        /// <typeparam cref="string">string</typeparam>
        public string GetCodeString()
        {
            return GetSnowflake().Code;
        }


        /// <summary>
        /// Static method
        /// Gets next Snowlflake as <typeparamref cref="ulong">ulong</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref>
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
        /// Gets next Snowlflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="ulong"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string GetCodeString(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetCodeString();
        }

        /// <summary>
        /// Gets next Snowlflake as <typeparamref cref="string">string</typeparamref> for a given <typeparamref cref="int"><paramref name="machineId"/></typeparamref>
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public static string GetCodeString(int machineId) => GetCodeString((ulong)machineId);
    }
}
