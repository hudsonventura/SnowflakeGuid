namespace SnowflakeID.Test
{
    public class SnowflakeIDGeneratorTest
    {
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
        public void SecuenciaTest(ulong machineId)
        {
            Queue<string> generados = new Queue<string>();
            int cant = 100000;
            DateTime d1 = DateTimeUtcMillis();
            for (int i = 0; i < cant; i++)
            {
                generados.Enqueue(SnowflakeIDGenerator.GetCodeString(machineId));
            }
            DateTime d2 = DateTimeUtcMillis();
            Snowflake anterior = Snowflake.Parse(generados.Dequeue());
            Assert.Multiple(() =>
            {
                Assert.That(anterior.MachineId, Is.EqualTo(machineId));
                Assert.That(d1, Is.LessThanOrEqualTo(anterior.UtcDateTime));
                Assert.That(d2, Is.GreaterThanOrEqualTo(anterior.UtcDateTime));
            });
            while (generados.Count > 0)
            {
                Snowflake actual = Snowflake.Parse(generados.Dequeue());
                Assert.Multiple(() =>
                {
                    Assert.That(actual.MachineId, Is.EqualTo(machineId));
                    Assert.That(anterior, Is.LessThan(actual));
                    Assert.That(d1, Is.LessThanOrEqualTo(actual.UtcDateTime));
                    Assert.That(d2, Is.GreaterThanOrEqualTo(actual.UtcDateTime));
                });
            }
        }


        [Test]
        public void ParallelTest()
        {
            List<Task<List<string>>> tasks = new List<Task<List<string>>>();
            const int cantidadTareas = 50;
            const int cantidadGenerados = 10000;
            const ulong machineId = 1ul;

            DateTime d1;
            DateTime d2;

            try
            {
                d1 = DateTimeUtcMillis();
                for (int i = 0; i < cantidadTareas; i++)
                {
                    tasks.Add(new Task<List<string>>(() =>
                    {
                        List<string> l = new List<string>();
                        for (int j = 0; j < cantidadGenerados; j++)
                        {
                            l.Add(SnowflakeIDGenerator.GetCodeString(machineId));
                        }
                        return l;
                    }));
                }
                foreach (Task task in tasks)
                {
                    task.Start();
                }

                Task.WaitAll(tasks.ToArray());
                d2 = DateTimeUtcMillis();
            }
            catch (AggregateException ae)
            {
                string err = string.Empty;
                foreach (Exception e in ae.InnerExceptions)
                {
                    string errL = $"Exception {e.GetType()} from {e.Source}.";
                    Console.WriteLine(errL);
                    err += " -|- " + errL;
                }
                Assert.Fail(err);
                throw;
            }

            IEnumerable<string> combinado = tasks.SelectMany(t => t.Result);

            Assert.Multiple(() =>
            {
                Assert.That(combinado.Count(), Is.EqualTo(cantidadTareas * cantidadGenerados));
                Assert.That(combinado.Distinct().Count(), Is.EqualTo(combinado.Count()));
            });
            foreach (string item in combinado)
            {
                Snowflake snowflake = Snowflake.Parse(item);
                Assert.Multiple(() =>
                {
                    Assert.That(d1, Is.LessThanOrEqualTo(snowflake.UtcDateTime));
                    Assert.That(d2, Is.GreaterThanOrEqualTo(snowflake.UtcDateTime));
                    Assert.That(snowflake.MachineId, Is.EqualTo(machineId));
                });
            }
        }


        [TestCase(1024UL)]
        [TestCase(1025UL)]
        [TestCase(2025UL)]
        [TestCase(20025UL)]
        public void EstacionErrorTest(ulong machineId)
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    SnowflakeIDGenerator.GetCode(machineId);
                });

                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    SnowflakeIDGenerator.GetCodeString(machineId);
                });

                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SnowflakeIDGenerator(machineId);
                });

                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SnowflakeIDGenerator((int)machineId);
                });
            });
        }


        [Test]
        public void DatetimeMillisTest()
        {
            // Esto prueba la función auxiliar de más abajo, que trunca la hora hasta milisegundo
            DateTime d1 = DateTime.UtcNow;
            DateTime d2 = DateTimeOnlyMillis(d1);
            Assert.Multiple(() =>
            {
                Assert.That(d1.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(d2.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(d2.Year, Is.EqualTo(d1.Year));
                Assert.That(d2.Month, Is.EqualTo(d1.Month));
                Assert.That(d2.Day, Is.EqualTo(d1.Day));
                Assert.That(d2.Hour, Is.EqualTo(d1.Hour));
                Assert.That(d2.Minute, Is.EqualTo(d1.Minute));
                Assert.That(d2.Second, Is.EqualTo(d1.Second));
                Assert.That(d2.Millisecond, Is.EqualTo(d1.Millisecond));
            });
        }






        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime DateTimeUtcMillis()
        {
            return DateTimeOnlyMillis(DateTime.UtcNow);
        }
        private static DateTime DateTimeOnlyMillis(DateTime d)
        {
            ulong ml = ((ulong)d.Subtract(UnixEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond); //equivalente a floor (división entera -> multiplicación)
            return DateTime.SpecifyKind(UnixEpoch.AddTicks((long)ml * (TimeSpan.TicksPerMillisecond)), DateTimeKind.Utc);
        }
    }
}