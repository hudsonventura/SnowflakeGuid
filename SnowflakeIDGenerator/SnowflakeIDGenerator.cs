using System;
using System.Threading;

[assembly: CLSCompliant(false)]
namespace SnowflakeID
{
    public class SnowflakeIDGenerator
    {
        private readonly ulong MACHINE_ID;
        //private readonly ulong APP_HASH;

        private static ulong _Sequence;
        private static ulong Sequence { get => _Sequence; set => _Sequence = value % Snowflake.MaxSequence; }

        private static readonly object lockObject = new();
        private static readonly DateTime epoch = new DateTime(1970, 1, 1);

        private static ulong ultimoTimestamp;





        public SnowflakeIDGenerator(ulong machineId)
        {
            if (machineId >= Snowflake.MaxMachineId)
            {
                throw new ArgumentOutOfRangeException(nameof(machineId), $"{nameof(machineId)} must be less than {Snowflake.MaxMachineId}. Got: {machineId}.");
            }
            MACHINE_ID = machineId;
        }
        public SnowflakeIDGenerator(int machineId) : this((ulong)machineId)
        { }

        public Snowflake GetSnowflake()
        {
            lock (lockObject)
            {
                ulong timestampActualMillis = ((ulong)DateTime.UtcNow.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);

                if (Sequence == 0 && timestampActualMillis == ultimoTimestamp)
                {
                    do
                    {
                        Thread.Sleep(1);
                        timestampActualMillis = ((ulong)DateTime.UtcNow.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
                    } while (timestampActualMillis == ultimoTimestamp);
                }
                else if (timestampActualMillis != ultimoTimestamp)
                {
                    Sequence = 0;
                }
                ultimoTimestamp = timestampActualMillis;

                Snowflake snowflake = new Snowflake()
                {
                    Timestamp = timestampActualMillis,
                    MachineId = MACHINE_ID,
                    Sequence = Sequence,
                };

                Sequence++;

                return snowflake;
            }
        }

        public ulong GetCode()
        {
            return GetSnowflake().Id;
        }

        public string GetCodeString()
        {
            return GetSnowflake().Code;
        }


        public static ulong GetCode(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetCode();
        }

        public static string GetCodeString(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetCodeString();
        }

    }
}
