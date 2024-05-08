using System;
using System.Globalization;

namespace SnowflakeID.Example.NetFramework
{
    internal class Application
    {
        protected readonly ISnowflakeIDGenerator _snowflakeIDGenerator;

        public Application(ISnowflakeIDGenerator snowflakeIDGenerator)
        {
            _snowflakeIDGenerator = snowflakeIDGenerator; //Injected
        }

        public void Run()
        {
            var snowflake = _snowflakeIDGenerator.GetSnowflake();

            Console.WriteLine($"server #: {_snowflakeIDGenerator.ConfiguredMachineId}");
            Console.WriteLine($"epoch #: {_snowflakeIDGenerator.ConfiguredEpoch.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)}");

            Console.WriteLine($"snowflake epoch #: {snowflake.Epoch.ToUniversalTime()}");
            Console.WriteLine($"snowflake server #: {snowflake.MachineId}");
            Console.WriteLine($"snowflake sequence #: {snowflake.Sequence}");

            Console.WriteLine($"resulting code: {snowflake.Code}");
        }
    }
}
