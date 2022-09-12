using System;
using System.Threading;

[assembly: CLSCompliant(false)]
namespace SnowflakeID
{
    public class SnowflakeIDGenerator
    {
        private readonly ulong MACHINE_ID;
        //private readonly ulong APP_HASH;

        private static ulong _Secuencia;
        private static ulong Secuencia { get => _Secuencia; set => _Secuencia = value % Snowflake.MaxSequence; }

        public static readonly object lockObject = new();
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

                if (Secuencia == 0 && timestampActualMillis == ultimoTimestamp)
                {
                    do
                    {
                        Thread.Sleep(1);
                        timestampActualMillis = ((ulong)DateTime.UtcNow.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
                    } while (timestampActualMillis == ultimoTimestamp);
                }
                else if (timestampActualMillis != ultimoTimestamp)
                {
                    Secuencia = 0;
                }
                ultimoTimestamp = timestampActualMillis;

                Snowflake snowflake = new Snowflake()
                {
                    Timestamp = timestampActualMillis,
                    MachineId = MACHINE_ID,
                    Secuencia = Secuencia,
                };

                Secuencia++;

                return snowflake;
            }
        }

        public ulong GetBarcode()
        {
            return GetSnowflake().Id;
        }

        public string GetBarcodeString()
        {
            return GetSnowflake().Barcode;
        }


        public static ulong GetBarcode(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetBarcode();
        }

        public static string GetBarcodeString(ulong machineId)
        {
            return new SnowflakeIDGenerator(machineId).GetBarcodeString();
        }

    }
}
