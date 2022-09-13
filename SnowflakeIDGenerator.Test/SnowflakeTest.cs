﻿namespace SnowflakeID.Test
{
    public class SnowflakeTest
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1);


        [SetUp]
        public void Setup()
        {
        }


        [TestCase(0ul)]
        [TestCase(1ul)]
        [TestCase(2ul)]
        [TestCase(3ul)]
        [TestCase(33ul)]
        [TestCase(1021ul)]
        [TestCase(1023ul)]
        public void ParseTest(ulong machineId)
        {
            string s = SnowflakeIDGenerator.GetCodeString(machineId);
            Snowflake ss = Snowflake.Parse(s);
            Assert.That(ss.Code, Is.EqualTo(s));
            Assert.Multiple(() =>
            {
                Assert.That(ss.MachineId, Is.EqualTo(machineId));
                Assert.That(ss.Id, Is.EqualTo(ulong.Parse(s)));
            });
            ulong b = SnowflakeIDGenerator.GetCode(machineId);
            Snowflake bs = Snowflake.Parse(b);
            Assert.Multiple(() =>
            {
                Assert.That(bs.Id, Is.EqualTo(b));
                Assert.That(bs.MachineId, Is.EqualTo(machineId));
                Assert.That(bs.Code, Is.EqualTo(b.ToString().PadLeft(Snowflake.NumberOfDigits, '0')));

                Assert.That(ss, Is.LessThan(bs));
                Assert.LessOrEqual(ss.Timestamp, bs.Timestamp);
                if (ss.Timestamp == bs.Timestamp)
                {
                    Assert.That(ss.Sequence, Is.LessThan(bs.Sequence));
                }
            });
        }

        [Test]
        public void CantidadDigitosTest()
        {
            //Esto debería fallar si cambian algo que no corresponda, jeje.
            Assert.That(Snowflake.NumberOfDigits, Is.EqualTo(20));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CrearTest(bool usarTimestamp, bool desc)
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            long i_ini = (long)(desc ? Snowflake.MaxMachineId - 1 : 0);
            long i_fin = (long)(desc ? 0 : Snowflake.MaxMachineId - 1);
            long j_ini = (long)(desc ? Snowflake.MaxSequence - 1 : 0);
            long j_fin = (long)(desc ? 0 : Snowflake.MaxSequence - 1);
            long step = desc ? -10 : 10;


            for (long i = i_ini; desc ? i >= i_fin : i <= i_fin; i += step)
            {
                for (long j = j_ini; desc ? j >= j_fin : j <= j_fin; j += step)
                {
                    Snowflake snowflake;
                    if (usarTimestamp)
                    {
                        snowflake = new Snowflake()
                        {
                            Timestamp = timestampActualMillis,
                            Sequence = (ulong)j,
                            MachineId = (ulong)i,
                        };
                    }
                    else
                    {
                        snowflake = new Snowflake()
                        {
                            UtcDateTime = d,
                            Sequence = (ulong)j,
                            MachineId = (ulong)i,
                        };
                    }
                    Assert.Multiple(() =>
                    {
                        Assert.That(snowflake.UtcDateTime.Year, Is.EqualTo(d.Year));
                        Assert.That(snowflake.UtcDateTime.Month, Is.EqualTo(d.Month));
                        Assert.That(snowflake.UtcDateTime.Day, Is.EqualTo(d.Day));
                        Assert.That(snowflake.UtcDateTime.Hour, Is.EqualTo(d.Hour));
                        Assert.That(snowflake.UtcDateTime.Minute, Is.EqualTo(d.Minute));
                        Assert.That(snowflake.UtcDateTime.Second, Is.EqualTo(d.Second));
                        Assert.That(snowflake.UtcDateTime.Millisecond, Is.EqualTo(d.Millisecond));
                        Assert.That(snowflake.Timestamp, Is.EqualTo(timestampActualMillis));
                        Assert.That(snowflake.MachineId, Is.EqualTo(i));
                        Assert.That(snowflake.Sequence, Is.EqualTo(j));
                    });

                    Snowflake parsedSnowflake = Snowflake.Parse(snowflake.Code);
                    Assert.Multiple(() =>
                    {
                        Assert.That(parsedSnowflake.UtcDateTime.Year, Is.EqualTo(d.Year));
                        Assert.That(parsedSnowflake.UtcDateTime.Month, Is.EqualTo(d.Month));
                        Assert.That(parsedSnowflake.UtcDateTime.Day, Is.EqualTo(d.Day));
                        Assert.That(parsedSnowflake.UtcDateTime.Hour, Is.EqualTo(d.Hour));
                        Assert.That(parsedSnowflake.UtcDateTime.Minute, Is.EqualTo(d.Minute));
                        Assert.That(parsedSnowflake.UtcDateTime.Second, Is.EqualTo(d.Second));
                        Assert.That(parsedSnowflake.UtcDateTime.Millisecond, Is.EqualTo(d.Millisecond));
                        Assert.That(parsedSnowflake.Timestamp, Is.EqualTo(timestampActualMillis));
                        Assert.That(parsedSnowflake.MachineId, Is.EqualTo(i));
                        Assert.That(parsedSnowflake.Sequence, Is.EqualTo(j));
                    });
                }
            }
        }


        [TestCase(1024UL, 4096UL)]
        [TestCase(124UL, 4096UL)]
        [TestCase(1024UL, 406UL)]
        [TestCase(2024UL, 4596UL)]
        [TestCase(224UL, 4596UL)]
        [TestCase(2024UL, 456UL)]
        public void CrearErrorTest(ulong machine, ulong sequence)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Snowflake snowflake = new Snowflake()
                {
                    UtcDateTime = DateTime.UtcNow,
                    MachineId = machine,
                    Sequence = sequence,
                };
            });
        }

    }
}