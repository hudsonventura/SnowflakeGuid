// Ignore Spelling: Rebase

using System.Globalization;

namespace SnowflakeID.Test
{
    public class SnowflakeTest
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static readonly DateTime defaultEpoch = DateTime.UnixEpoch;
#else
        // This should be DateTime.UnixEpoch. However, that constant is only available in .netCore and net5 or newer
        private static readonly DateTime defaultEpoch = new(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
#endif
        private static readonly DateTime customEpoch = new(year: 2023, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);


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
        public void ParseTest(ulong machineId)
        {
            string s = SnowflakeIDGenerator.GetCodeString(machineId);
            Snowflake ss = Snowflake.Parse(s);
            Assert.Multiple(() =>
            {
                Assert.That(ss.Code, Is.EqualTo(s));
                Assert.That(ss.MachineId, Is.EqualTo(machineId));
                Assert.That(ss.Id, Is.EqualTo(ulong.Parse(s, CultureInfo.InvariantCulture)));
            });
            ulong b = SnowflakeIDGenerator.GetCode(machineId);
            Snowflake bs = Snowflake.Parse(b);
            Assert.Multiple(() =>
            {
                Assert.That(bs.Id, Is.EqualTo(b));
                Assert.That(bs.MachineId, Is.EqualTo(machineId));
                Assert.That(bs.Code, Is.EqualTo(b.ToString(CultureInfo.InvariantCulture).PadLeft(Snowflake.NumberOfDigits, '0')));

                Assert.That(ss, Is.LessThan(bs));
                Assert.That(ss.Timestamp, Is.LessThanOrEqualTo(bs.Timestamp));
                if (ss.Timestamp == bs.Timestamp)
                {
                    Assert.That(ss.Sequence, Is.LessThan(bs.Sequence));
                }
            });
        }

        [Test]
        public void DigitAmountTest()
        {
            //This should fail if someone touches something they shouldn't
            Assert.That(Snowflake.NumberOfDigits, Is.EqualTo(20));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CreationTest(bool useTimeStamp, bool desc)
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(defaultEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            long i_ini = (desc ? Snowflake.MaxMachineId - 1 : 0);
            long i_fin = (desc ? 0 : Snowflake.MaxMachineId - 1);
            long j_ini = (desc ? Snowflake.MaxSequence - 1 : 0);
            long j_fin = (desc ? 0 : Snowflake.MaxSequence - 1);
            long step = desc ? -10 : 10;


            for (long i = i_ini; desc ? i >= i_fin : i <= i_fin; i += step)
            {
                for (long j = j_ini; desc ? j >= j_fin : j <= j_fin; j += step)
                {
                    Snowflake snowflake;
                    if (useTimeStamp)
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
        public void CreationErrorTest(ulong machine, ulong sequence)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new Snowflake()
                {
                    UtcDateTime = DateTime.UtcNow,
                    MachineId = machine,
                    Sequence = sequence,
                };
            });
        }

        [TestCase(1024, 4096)]
        [TestCase(124, 4096)]
        [TestCase(1024, 406)]
        [TestCase(2024, 4596)]
        [TestCase(224, 4596)]
        [TestCase(2024, 456)]
        [TestCase(124, -1)]
        [TestCase(-1, 406)]
        [TestCase(-2024, -4596)]
        [TestCase(-1, -1)]
        public void CreationErrorTest(int machine, int sequence)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new Snowflake()
                {
                    UtcDateTime = DateTime.UtcNow,
                    MachineIdInt32 = machine,
                    SequenceInt32 = sequence,
                };
            });
        }

        [TestCase(-1)]
        [TestCase(0b0000_0000_0000_0000_0000_0111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111)]
        [TestCase(8796093022207)]
        [TestCase(0b0000_0000_0000_0000_0000_0100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
        [TestCase(4398046511104)]
        [TestCase(0b0000_0000_0010_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000_0010_0000)]
        [TestCase(9007199254872096)]
        public void CreationErrorTest(long timestamp)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new Snowflake()
                {
                    TimestampInt64 = timestamp,
                    MachineIdInt32 = 123,
                    SequenceInt32 = 123,
                };
            });
        }


        [TestCase(0ul, 0ul)]
        [TestCase(0ul, 4095ul)]
        [TestCase(1ul, 542ul)]
        [TestCase(2ul, 42ul)]
        [TestCase(3ul, 52ul)]
        [TestCase(33ul, 3442ul)]
        [TestCase(1021ul, 1542ul)]
        [TestCase(1023ul, 2542ul)]
        public void Date(ulong machineId, ulong sequence)
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(defaultEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            DateTime dMillis = DateTime.SpecifyKind(defaultEpoch.AddMilliseconds(timestampActualMillis), DateTimeKind.Utc);

            Assert.Multiple(() =>
            {
                Assert.That(dMillis.Year, Is.EqualTo(d.Year));
                Assert.That(dMillis.Month, Is.EqualTo(d.Month));
                Assert.That(dMillis.Day, Is.EqualTo(d.Day));
                Assert.That(dMillis.DayOfWeek, Is.EqualTo(d.DayOfWeek));
                Assert.That(dMillis.Hour, Is.EqualTo(d.Hour));
                Assert.That(dMillis.Minute, Is.EqualTo(d.Minute));
                Assert.That(dMillis.Second, Is.EqualTo(d.Second));
                Assert.That(dMillis.Millisecond, Is.EqualTo(d.Millisecond));
                Assert.That(dMillis.Kind, Is.EqualTo(d.Kind));
            });

            Snowflake snowflakeDateTime = new()
            {
                UtcDateTime = dMillis,
                MachineId = machineId,
                Sequence = sequence,
            };
            Snowflake snowflakeTimestamp = new()
            {
                Timestamp = timestampActualMillis,
                MachineId = machineId,
                Sequence = sequence,
            };

            Assert.Multiple(() =>
            {
                Assert.That(snowflakeTimestamp.Sequence, Is.EqualTo(snowflakeDateTime.Sequence));
                Assert.That(snowflakeTimestamp.Timestamp, Is.EqualTo(snowflakeDateTime.Timestamp));
                Assert.That(snowflakeTimestamp.UtcDateTime, Is.EqualTo(snowflakeDateTime.UtcDateTime));
                Assert.That(snowflakeTimestamp.MachineId, Is.EqualTo(snowflakeDateTime.MachineId));
                Assert.That(snowflakeTimestamp.Id, Is.EqualTo(snowflakeDateTime.Id));
                Assert.That(snowflakeTimestamp.Code, Is.EqualTo(snowflakeDateTime.Code));
                Assert.That(snowflakeTimestamp, Is.EqualTo(snowflakeDateTime));
            });
        }


        [Test]
        public void UnsignedEqualsSigned()
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(defaultEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            ulong machineId = 123;
            ulong sequence = 3468;
            int machineIdInt32 = (int)machineId;
            int sequenceInt32 = (int)sequence;
            long timestampActualMillisInt64 = (long)timestampActualMillis;

            Snowflake snowflake = new()
            {
                Timestamp = timestampActualMillis,
                MachineId = machineId,
                Sequence = sequence,
            };

            Snowflake snowflakeCLS = new()
            {
                TimestampInt64 = timestampActualMillisInt64,
                MachineIdInt32 = machineIdInt32,
                SequenceInt32 = sequenceInt32,
            };

            Assert.Multiple(() =>
            {
                Assert.That(snowflake.MachineIdInt32, Is.EqualTo(snowflake.MachineId));
                Assert.That(snowflake.SequenceInt32, Is.EqualTo(snowflake.Sequence));
                Assert.That(snowflake.TimestampInt64, Is.EqualTo(snowflake.Timestamp));
                Assert.That(ulong.Parse(snowflake.Code, CultureInfo.InvariantCulture), Is.EqualTo(snowflake.Id));

                Assert.That(snowflakeCLS.MachineIdInt32, Is.EqualTo(snowflakeCLS.MachineId));
                Assert.That(snowflakeCLS.SequenceInt32, Is.EqualTo(snowflakeCLS.Sequence));
                Assert.That(snowflakeCLS.TimestampInt64, Is.EqualTo(snowflakeCLS.Timestamp));
                Assert.That(ulong.Parse(snowflakeCLS.Code, CultureInfo.InvariantCulture), Is.EqualTo(snowflakeCLS.Id));

                Assert.That(snowflakeCLS, Is.EqualTo(snowflake));
                Assert.That(snowflakeCLS.Code, Is.EqualTo(snowflake.Code));
                Assert.That(snowflakeCLS.Id, Is.EqualTo(snowflake.Id));
                Assert.That(snowflakeCLS.MachineIdInt32, Is.EqualTo(snowflake.MachineIdInt32));
                Assert.That(snowflakeCLS.SequenceInt32, Is.EqualTo(snowflake.SequenceInt32));
                Assert.That(snowflakeCLS.TimestampInt64, Is.EqualTo(snowflake.TimestampInt64));
                Assert.That(snowflakeCLS.MachineId, Is.EqualTo(snowflake.MachineId));
                Assert.That(snowflakeCLS.Sequence, Is.EqualTo(snowflake.Sequence));
                Assert.That(snowflakeCLS.Timestamp, Is.EqualTo(snowflake.Timestamp));

                Assert.That(SnowflakeIDGenerator.GetCode(123), Is.LessThan(ulong.Parse(SnowflakeIDGenerator.GetCodeString(123), CultureInfo.InvariantCulture)));
            });
        }

        [Test]
        public void DefaultEpochTest()
        {
            DateTime d = DateTime.UtcNow;

            Snowflake snowflakeDefault = new()
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };

            Snowflake snowflakeCustomEqualDefault = new(defaultEpoch)
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };

            Assert.Multiple(() =>
            {
                Assert.That(snowflakeDefault.Epoch, Is.EqualTo(defaultEpoch));
                Assert.That(snowflakeDefault.Epoch, Is.Not.EqualTo(customEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.EqualTo(defaultEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.Not.EqualTo(customEpoch));
                Assert.That(snowflakeDefault, Is.EqualTo(snowflakeCustomEqualDefault));
                Assert.That(snowflakeDefault.Epoch, Is.EqualTo(snowflakeCustomEqualDefault.Epoch));
                Assert.That(snowflakeDefault.MachineId, Is.EqualTo(snowflakeCustomEqualDefault.MachineId));
                Assert.That(snowflakeDefault.Sequence, Is.EqualTo(snowflakeCustomEqualDefault.Sequence));
                Assert.That(snowflakeDefault.Timestamp, Is.EqualTo(snowflakeCustomEqualDefault.Timestamp));
                Assert.That(snowflakeDefault.UtcDateTime, Is.EqualTo(snowflakeCustomEqualDefault.UtcDateTime));
                Assert.That(snowflakeDefault.Code, Is.EqualTo(snowflakeCustomEqualDefault.Code));
                Assert.That(snowflakeDefault.ToString(), Is.EqualTo(snowflakeCustomEqualDefault.ToString()));
            });
        }

        [Test]
        public void CustomEpochTest()
        {
            DateTime d = DateTime.UtcNow;

            Snowflake snowflakeDefault = new()
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };

            Snowflake snowflakeCustomEqualDefault = new(customEpoch)
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };

            Assert.Multiple(() =>
            {
                Assert.That(snowflakeDefault.Epoch, Is.EqualTo(defaultEpoch));
                Assert.That(snowflakeDefault.Epoch, Is.Not.EqualTo(customEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.Not.EqualTo(defaultEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.EqualTo(customEpoch));
                Assert.That(snowflakeDefault, Is.Not.EqualTo(snowflakeCustomEqualDefault));
                Assert.That(snowflakeDefault.Epoch, Is.Not.EqualTo(snowflakeCustomEqualDefault.Epoch));
                Assert.That(snowflakeDefault.MachineId, Is.EqualTo(snowflakeCustomEqualDefault.MachineId));
                Assert.That(snowflakeDefault.Sequence, Is.EqualTo(snowflakeCustomEqualDefault.Sequence));
                Assert.That(snowflakeDefault.Timestamp, Is.Not.EqualTo(snowflakeCustomEqualDefault.Timestamp));
                Assert.That(snowflakeDefault.UtcDateTime, Is.EqualTo(snowflakeCustomEqualDefault.UtcDateTime));
                Assert.That(snowflakeDefault.Code, Is.Not.EqualTo(snowflakeCustomEqualDefault.Code));
                Assert.That(snowflakeDefault.ToString(), Is.Not.EqualTo(snowflakeCustomEqualDefault.ToString()));
            });
        }

        private static IEnumerable<TestCaseData> ChangeEpochTestCaseData()
        {
            yield return new TestCaseData(defaultEpoch, defaultEpoch);
            yield return new TestCaseData(defaultEpoch, customEpoch);
            yield return new TestCaseData(customEpoch, defaultEpoch);
            yield return new TestCaseData(customEpoch, customEpoch);
        }

        [TestCaseSource(nameof(ChangeEpochTestCaseData))]
        public void ChangeEpoch(DateTime oldEpoch, DateTime newEpoch)
        {
            DateTime dateTime = DateTime.UtcNow;
            ulong seq = 12;
            ulong machine = 65;

            TimeSpan epochDifference = newEpoch - oldEpoch;

            Snowflake snowflake = new(epoch: oldEpoch)
            {
                UtcDateTime = dateTime,
                Sequence = seq,
                MachineId = machine,
            };
            string oldCode = snowflake.Code;

            Assert.That(snowflake.Epoch, Is.EqualTo(oldEpoch));

            snowflake.ChangeEpoch(newEpoch);

            Assert.Multiple(() =>
            {
                Assert.That(snowflake.UtcDateTime, Is.EqualTo(dateTime + epochDifference));
                Assert.That(snowflake.Sequence, Is.EqualTo(seq));
                Assert.That(snowflake.MachineId, Is.EqualTo(machine));
                Assert.That(snowflake.Epoch, Is.EqualTo(newEpoch));
                Assert.That(snowflake.Code, Is.EqualTo(oldCode));
            });
        }

        [TestCaseSource(nameof(ChangeEpochTestCaseData))]
        public void RebaseEpoch(DateTime oldEpoch, DateTime newEpoch)
        {
            DateTime dateTime = DateTime.UtcNow;
            ulong seq = 12;
            ulong machine = 65;

            Snowflake snowflake = new(epoch: oldEpoch)
            {
                UtcDateTime = dateTime,
                Sequence = seq,
                MachineId = machine,
            };
            string oldCode = snowflake.Code;

            Assert.That(snowflake.Epoch, Is.EqualTo(oldEpoch));

            snowflake.RebaseEpoch(newEpoch);

            Assert.Multiple(() =>
            {
                Assert.That(snowflake.UtcDateTime, Is.EqualTo(dateTime));
                Assert.That(snowflake.Sequence, Is.EqualTo(seq));
                Assert.That(snowflake.MachineId, Is.EqualTo(machine));
                Assert.That(snowflake.Epoch, Is.EqualTo(newEpoch));
                if (oldEpoch == newEpoch)
                {
                    Assert.That(snowflake.Code, Is.EqualTo(oldCode));
                }
                else
                {
                    Assert.That(snowflake.Code, Is.Not.EqualTo(oldCode));
                }
            });
        }

        [Test]
        public void SameCodeDifferentSnowflakeTest()
        {
            DateTime d = DateTime.UtcNow;

            Snowflake snowflakeDefault = new()
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };

            Snowflake snowflakeCustomEqualDefault = new()
            {
                UtcDateTime = d,
                MachineId = 123,
                Sequence = 456,
            };
            snowflakeCustomEqualDefault.ChangeEpoch(customEpoch);

            Assert.Multiple(() =>
            {
                Assert.That(snowflakeCustomEqualDefault, Is.Not.EqualTo(snowflakeDefault));
                Assert.That(snowflakeDefault.Epoch, Is.EqualTo(defaultEpoch));
                Assert.That(snowflakeDefault.Epoch, Is.Not.EqualTo(customEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.Not.EqualTo(defaultEpoch));
                Assert.That(snowflakeCustomEqualDefault.Epoch, Is.EqualTo(customEpoch));
                Assert.That(snowflakeDefault, Is.Not.EqualTo(snowflakeCustomEqualDefault));
                Assert.That(snowflakeDefault.Epoch, Is.Not.EqualTo(snowflakeCustomEqualDefault.Epoch));
                Assert.That(snowflakeDefault.MachineId, Is.EqualTo(snowflakeCustomEqualDefault.MachineId));
                Assert.That(snowflakeDefault.Sequence, Is.EqualTo(snowflakeCustomEqualDefault.Sequence));
                Assert.That(snowflakeDefault.Timestamp, Is.EqualTo(snowflakeCustomEqualDefault.Timestamp));
                Assert.That(snowflakeDefault.UtcDateTime, Is.Not.EqualTo(snowflakeCustomEqualDefault.UtcDateTime));
                Assert.That(snowflakeDefault.Code, Is.EqualTo(snowflakeCustomEqualDefault.Code));
                Assert.That(snowflakeDefault.ToString(), Is.EqualTo(snowflakeCustomEqualDefault.ToString()));
            });
        }

        [Test]
        public void CastOperators()
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(defaultEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            DateTime dateTime = DateTime.SpecifyKind(defaultEpoch.AddMilliseconds(timestampActualMillis), DateTimeKind.Utc); // remove sub-millisecond precision
            ulong seq = 12;
            ulong machine = 65;

            Snowflake snowflake = new()
            {
                UtcDateTime = dateTime,
                Sequence = seq,
                MachineId = machine,
            };

            ulong number = snowflake;
            Assert.That(number, Is.EqualTo(snowflake.Id));

            string code = snowflake;
            Assert.That(code, Is.EqualTo(snowflake.Code));

            Snowflake fromNumber = (Snowflake)number;
            Assert.Multiple(() =>
            {
                Assert.That(fromNumber.Id, Is.EqualTo(number));
                Assert.That(fromNumber, Is.EqualTo(snowflake));
                Assert.That(fromNumber.UtcDateTime, Is.EqualTo(snowflake.UtcDateTime));
                Assert.That(fromNumber.Sequence, Is.EqualTo(snowflake.Sequence));
                Assert.That(fromNumber.MachineId, Is.EqualTo(snowflake.MachineId));
            });

            Snowflake fromCode = (Snowflake)code;
            Assert.Multiple(() =>
            {
                Assert.That(fromCode.Code, Is.EqualTo(code));
                Assert.That(fromCode, Is.EqualTo(snowflake));
                Assert.That(fromCode.UtcDateTime, Is.EqualTo(snowflake.UtcDateTime));
                Assert.That(fromCode.Sequence, Is.EqualTo(snowflake.Sequence));
                Assert.That(fromCode.MachineId, Is.EqualTo(snowflake.MachineId));
            });
        }

        [Test]
        public void CastAlternativeOperators()
        {
            DateTime d = DateTime.UtcNow;
            ulong timestampActualMillis = ((ulong)d.Subtract(defaultEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            DateTime dateTime = DateTime.SpecifyKind(defaultEpoch.AddMilliseconds(timestampActualMillis), DateTimeKind.Utc); // remove sub-millisecond precision
            ulong seq = 12;
            ulong machine = 65;

            Snowflake snowflake = new()
            {
                UtcDateTime = dateTime,
                Sequence = seq,
                MachineId = machine,
            };

            ulong number = snowflake.ToUInt64();
            Assert.That(number, Is.EqualTo(snowflake.Id));

            string code = snowflake.ToString();
            Assert.That(code, Is.EqualTo(snowflake.Code));

            Snowflake fromNumber = Snowflake.FromUInt64(number);
            Assert.Multiple(() =>
            {
                Assert.That(fromNumber.Id, Is.EqualTo(number));
                Assert.That(fromNumber, Is.EqualTo(snowflake));
                Assert.That(fromNumber.UtcDateTime, Is.EqualTo(snowflake.UtcDateTime));
                Assert.That(fromNumber.Sequence, Is.EqualTo(snowflake.Sequence));
                Assert.That(fromNumber.MachineId, Is.EqualTo(snowflake.MachineId));
            });

            Snowflake fromCode = Snowflake.FromString(code);
            Assert.Multiple(() =>
            {
                Assert.That(fromCode.Code, Is.EqualTo(code));
                Assert.That(fromCode, Is.EqualTo(snowflake));
                Assert.That(fromCode.UtcDateTime, Is.EqualTo(snowflake.UtcDateTime));
                Assert.That(fromCode.Sequence, Is.EqualTo(snowflake.Sequence));
                Assert.That(fromCode.MachineId, Is.EqualTo(snowflake.MachineId));
            });
        }

        [Test]
        public void MaxTimestamp()
        {
            ulong ts = (ulong)Math.Pow(2, 42) - 1;

            Snowflake s = new()
            {
                Timestamp = ts,
                MachineId = 0,
                Sequence = 0,
            };

            string c = s.ToString();

            Snowflake snowflake = Snowflake.Parse(c);
            string c2 = snowflake.ToString();

            Assert.Multiple(() =>
            {
                Assert.That(snowflake, Is.EqualTo(s));
                Assert.That(c, Is.EqualTo(c2));
                Assert.That(snowflake.Sequence, Is.EqualTo(s.Sequence));
                Assert.That(snowflake.MachineId, Is.EqualTo(s.MachineId));
                Assert.That(snowflake.Timestamp, Is.EqualTo(s.Timestamp));
                Assert.That(snowflake.TimestampInt64, Is.EqualTo(s.TimestampInt64));
                Assert.That(snowflake.UtcDateTime, Is.EqualTo(s.UtcDateTime));
            });
        }

        [Test]
        public void TimestampOutOfRange()
        {
            ulong ts = (ulong)Math.Pow(2, 42);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _ = new Snowflake()
                {
                    Timestamp = ts,
                    MachineId = 0,
                    Sequence = 0,
                };
            });
        }
    }
}