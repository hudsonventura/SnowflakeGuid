using Autofac;
using System;
using System.Globalization;

namespace SnowflakeID.Example.NetFramework
{
    internal static class Program
    {
        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SnowflakeIDGenerator>()
                .As<ISnowflakeIDGenerator>()
                .UsingConstructor(typeof(int), typeof(DateTime))
                .WithParameter("machineId", 22)
                .WithParameter("customEpoch", DateTime.Parse("2024-05-07", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
            builder.RegisterType<Application>();
            return builder.Build();
        }

        static void Main(string[] args)
        {
            CompositionRoot().Resolve<Application>().Run();
        }
    }
}
