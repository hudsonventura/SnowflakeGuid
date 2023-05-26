namespace SnowflakeID.Test
{
    public class SnowflakeIDGeneratorTest
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [TestCase(0ul)]
        [TestCase(1ul)]
        [TestCase(2ul)]
        [TestCase(3ul)]
        [TestCase(33ul)]
        [TestCase(1021ul)]
        [TestCase(1023ul)]
        public void SequenceTest(ulong machineId)
        {
            Queue<string> generated = new();
            int cant = 100000;
            DateTime d1 = DateTimeUtcMillis();
            for (int i = 0; i < cant; i++)
            {
                generated.Enqueue(SnowflakeIDGenerator.GetCodeString(machineId));
            }
            DateTime d2 = DateTimeUtcMillis();
            Snowflake previous = Snowflake.Parse(generated.Dequeue());
            Assert.Multiple(() =>
            {
                Assert.That(previous.MachineId, Is.EqualTo(machineId));
                Assert.That(d1, Is.LessThanOrEqualTo(previous.UtcDateTime));
                Assert.That(d2, Is.GreaterThanOrEqualTo(previous.UtcDateTime));
            });

            Snowflake? prevSnowflake = null;
            while (generated.Count > 0)
            {
                Snowflake current = Snowflake.Parse(generated.Dequeue());
                Assert.Multiple(() =>
                {
                    Assert.That(current.MachineId, Is.EqualTo(machineId));
                    Assert.That(previous, Is.LessThan(current));
                    Assert.That(d1, Is.LessThanOrEqualTo(current.UtcDateTime));
                    Assert.That(d2, Is.GreaterThanOrEqualTo(current.UtcDateTime));

                    if (prevSnowflake != null)
                    {
                        Assert.That(current, Is.GreaterThanOrEqualTo(prevSnowflake));
                        Assert.That(current >= prevSnowflake, Is.True); // testing operators
                        Assert.That(current, Is.GreaterThan(prevSnowflake));
                        Assert.That(current > prevSnowflake, Is.True); // testing operators
                    }
                    Assert.That(current <= prevSnowflake, Is.False); // testing operators
                    Assert.That(current < prevSnowflake, Is.False); // testing operators
                });

                prevSnowflake = current;
            }
        }


        [Test]
        public void ParallelTest()
        {
            List<Task<List<string>>> tasks = new();
            const int TaskQuantity = 50;
            const int GeneratedQuantity = 10000;
            const ulong machineId = 1ul;

            DateTime d1;
            DateTime d2;

            try
            {
                d1 = DateTimeUtcMillis();
                for (int i = 0; i < TaskQuantity; i++)
                {
                    tasks.Add(new Task<List<string>>(() =>
                    {
                        List<string> l = new();
                        for (int j = 0; j < GeneratedQuantity; j++)
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

            IEnumerable<string> combined = tasks.SelectMany(t => t.Result);

            Assert.Multiple(() =>
            {
                Assert.That(combined.Count(), Is.EqualTo(TaskQuantity * GeneratedQuantity));
                Assert.That(combined.Distinct().Count(), Is.EqualTo(combined.Count()));
            });
            foreach (string item in combined)
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
        public void MachineErrorTest(ulong machineId)
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
        public void DefaultEpochTest()
        {
            ulong code = SnowflakeIDGenerator.GetCode(123);
            ulong codeDefaultEpochSetted = SnowflakeIDGenerator.GetCode(123, UnixEpoch);

            Snowflake snowflake = Snowflake.Parse(code);
            Snowflake snowflakeDefaultEpochSetted = Snowflake.Parse(codeDefaultEpochSetted, UnixEpoch);

            Assert.Multiple(() =>
            {
                Assert.That(snowflake, Is.LessThan(snowflakeDefaultEpochSetted));
                Assert.That(snowflake.Epoch, Is.EqualTo(snowflakeDefaultEpochSetted.Epoch));
                Assert.That(snowflake.Timestamp, Is.LessThanOrEqualTo(snowflakeDefaultEpochSetted.Timestamp));
            });
        }

        [Test]
        public void CustomTest()
        {
            ulong code = SnowflakeIDGenerator.GetCode(123);
            ulong codeDefaultEpochSetted = SnowflakeIDGenerator.GetCode(123, CustomEpoch);

            Snowflake snowflake = Snowflake.Parse(code);
            Snowflake snowflakeDefaultEpochSetted = Snowflake.Parse(codeDefaultEpochSetted, CustomEpoch);

            Assert.Multiple(() =>
            {
                Assert.That(snowflake, Is.GreaterThan(snowflakeDefaultEpochSetted));
                Assert.That(snowflake.Epoch, Is.Not.EqualTo(snowflakeDefaultEpochSetted.Epoch));
                Assert.That(snowflake.Epoch, Is.EqualTo(UnixEpoch));
                Assert.That(snowflakeDefaultEpochSetted.Epoch, Is.EqualTo(CustomEpoch));
                Assert.That(snowflakeDefaultEpochSetted.Timestamp, Is.LessThan(snowflake.Timestamp));
            });
        }


        [Test]
        public void DateTimeMillisecondsTest()
        {
            // This tests aux function bellow. It truncates time to a millisecond
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

        [Test]
        public void TimestampMillisFromEpochTest()
        {
            DateTime d = DateTime.UtcNow;
            ulong oldWay = ((ulong)d.Subtract(UnixEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            ulong newWay = SnowflakeIDGenerator.TimestampMillisFromEpoch(d, UnixEpoch);
            Assert.That(newWay, Is.EqualTo(oldWay));
        }

        [Test]
        public void DriftTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SnowflakeIDGenerator.TimestampMillisFromEpoch(UnixEpoch, UnixEpoch), Is.EqualTo(0));
                Assert.That(SnowflakeIDGenerator.TimestampMillisFromEpoch(CustomEpoch, UnixEpoch), Is.EqualTo(1577836800000));
            });
        }






        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime CustomEpoch = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime DateTimeUtcMillis()
        {
            return DateTimeOnlyMillis(DateTime.UtcNow);
        }
        private static DateTime DateTimeOnlyMillis(DateTime d)
        {
            ulong ml = ((ulong)d.Subtract(UnixEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond); // same as floor (integer division -> multiplication)
            return DateTime.SpecifyKind(UnixEpoch.AddTicks((long)ml * (TimeSpan.TicksPerMillisecond)), DateTimeKind.Utc);
        }
    }
}