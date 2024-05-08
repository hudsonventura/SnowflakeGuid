#if NET6_0_OR_GREATER
using System.Globalization;

namespace SnowflakeID.Test
{
    public class SnowflakeIdGeneratorOptionsTest
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [Test]
        public void DefaultValuesTest()
        {
            SnowflakeIdGeneratorOptions obj = new();
            Assert.Multiple(() =>
            {
                Assert.That(obj.MachineId, Is.EqualTo(0));
                Assert.That(obj.EpochObject, Is.EqualTo(DateTime.UnixEpoch));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTime.UnixEpoch.Kind));
                Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(DateTime.UnixEpoch.ToUniversalTime()));
                Assert.That(obj.Epoch, Is.EqualTo(DateTime.UnixEpoch.ToString("O")));
            });
        }

        [TestCase(4, "1970-01-01 00:00:00")]
        [TestCase(0, "1970-01-01 00:00:00")]
        [TestCase(10, "2000-01-01 00:00:00")]
        [TestCase(6, null)]
        [TestCase(1, null)]
        [TestCase(1, "2020-04-19 00:25:00")]
        [TestCase(null, "1970-01-01")]
        [TestCase(0, "1970-01-01")]
        [TestCase(10, "2000-01-01")]
        [TestCase(6, "1970-01-01")]
        [TestCase(1, "1970-01-01")]
        [TestCase(null, "2020-04-19")]
        public void CustomValuesTest(int? machineId, string? epoch)
        {
            SnowflakeIdGeneratorOptions obj = new();
            if (machineId != null) { obj.MachineId = machineId.Value; }
            if (epoch != null) { obj.Epoch = epoch; }

            if (machineId != null)
            {
                Assert.That(obj.MachineId, Is.EqualTo(machineId));
            }
            else
            {
                Assert.That(obj.MachineId, Is.EqualTo(0));
            }

            if (!string.IsNullOrWhiteSpace(epoch))
            {
                DateTime epochObject = DateTime.Parse(epoch, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite).ToUniversalTime();
                Assert.Multiple(() =>
                {
                    Assert.That(obj.EpochObject, Is.EqualTo(epochObject));
                    Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                    Assert.That(obj.EpochObject.Kind, Is.EqualTo(epochObject.Kind));
                    Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(epochObject.ToUniversalTime()));
                    Assert.That(obj.Epoch, Is.EqualTo(epochObject.ToString("O")));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(obj.EpochObject, Is.EqualTo(DateTime.UnixEpoch));
                    Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                    Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTime.UnixEpoch.Kind));
                    Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(DateTime.UnixEpoch.ToUniversalTime()));
                    Assert.That(obj.Epoch, Is.EqualTo(DateTime.UnixEpoch.ToString("O")));
                });
            }
        }
    }
}
#endif
