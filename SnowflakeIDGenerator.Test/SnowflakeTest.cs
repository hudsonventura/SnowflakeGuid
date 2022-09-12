namespace SnowflakeID.Test
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
        public void ParseTest(ulong idEstacion)
        {
            string s = SnowflakeIDGenerator.GetBarcodeString(idEstacion);
            Snowflake ss = Snowflake.Parse(s);
            Assert.That(ss.Barcode, Is.EqualTo(s));
            Assert.Multiple(() =>
            {
                Assert.That(ss.MachineId, Is.EqualTo(idEstacion));
                Assert.That(ss.Id, Is.EqualTo(ulong.Parse(s)));
            });
            ulong b = SnowflakeIDGenerator.GetBarcode(idEstacion);
            Snowflake bs = Snowflake.Parse(b);
            Assert.Multiple(() =>
            {
                Assert.That(bs.Id, Is.EqualTo(b));
                Assert.That(bs.MachineId, Is.EqualTo(idEstacion));
                Assert.That(bs.Barcode, Is.EqualTo(b.ToString().PadLeft(Snowflake.CantidadDigitos, '0')));

                Assert.That(ss, Is.LessThan(bs));
                Assert.LessOrEqual(ss.Timestamp, bs.Timestamp);
                if (ss.Timestamp == bs.Timestamp)
                {
                    Assert.That(ss.Secuencia, Is.LessThan(bs.Secuencia));
                }
            });
        }

        [Test]
        public void CantidadDigitosTest()
        {
            //Esto debería fallar si cambian algo que no corresponda, jeje.
            Assert.That(Snowflake.CantidadDigitos, Is.EqualTo(20));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CrearTest(bool usarTimestamp, bool desc)
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            long i_ini = (long)(desc ? Snowflake.MAXIMO_TERMINAL - 1 : 0);
            long i_fin = (long)(desc ? 0 : Snowflake.MAXIMO_TERMINAL - 1);
            long j_ini = (long)(desc ? Snowflake.MAXIMO_SECUENCIA - 1 : 0);
            long j_fin = (long)(desc ? 0 : Snowflake.MAXIMO_SECUENCIA - 1);
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
                            Secuencia = (ulong)j,
                            MachineId = (ulong)i,
                        };
                    }
                    else
                    {
                        snowflake = new Snowflake()
                        {
                            UtcDateTime = d,
                            Secuencia = (ulong)j,
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
                        Assert.That(snowflake.Secuencia, Is.EqualTo(j));
                    });

                    Snowflake parsedSnowflake = Snowflake.Parse(snowflake.Barcode);
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
                        Assert.That(parsedSnowflake.Secuencia, Is.EqualTo(j));
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
        public void CrearErrorTest(ulong terminal, ulong secuencia)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Snowflake snowflake = new Snowflake()
                {
                    UtcDateTime = DateTime.UtcNow,
                    MachineId = terminal,
                    Secuencia = secuencia,
                };
            });
        }

    }
}