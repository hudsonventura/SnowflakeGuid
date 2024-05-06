#if NET6_0_OR_GREATER
using SnowflakeID.DependencyInjection;
using System.Globalization;

namespace SnowflakeID.Test
{
    internal class SnowflakeIdGeneratorOptionsTest
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
                Assert.That(obj.MachineId, Is.EqualTo(1));
                Assert.That(obj.EpochObject, Is.EqualTo(DateTime.UnixEpoch));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTime.UnixEpoch.Kind));
                Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(DateTime.UnixEpoch.ToUniversalTime()));
                Assert.That(obj.Epoch, Is.EqualTo(DateTime.UnixEpoch.ToString("O")));
            });
        }

        [TestCase(4ul, "1970-01-01 00:00:00")]
        [TestCase(0ul, "1970-01-01 00:00:00")]
        [TestCase(10ul, "2000-01-01 00:00:00")]
        [TestCase(6ul, null)]
        [TestCase(1ul, null)]
        [TestCase(1ul, "2020-04-19 00:25:00")]
        [TestCase(null, "1970-01-01")]
        [TestCase(0ul, "1970-01-01")]
        [TestCase(10ul, "2000-01-01")]
        [TestCase(6ul, "1970-01-01")]
        [TestCase(1ul, "1970-01-01")]
        [TestCase(null, "2020-04-19")]
        public void CustomValuesTest(ulong? mId, string epoch)
        {
            SnowflakeIdGeneratorOptions obj = new();
            if (mId != null) { obj.MachineId = mId.Value; }
            if (epoch != null) { obj.Epoch = epoch; }

            if (mId != null)
            {
                Assert.That(obj.MachineId, Is.EqualTo(mId));
            }
            else
            {
                Assert.That(obj.MachineId, Is.EqualTo(1));
            }

            if (!string.IsNullOrWhiteSpace(epoch))
            {
                DateTime epochObject = DateTime.Parse(epoch, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite).ToUniversalTime();
                Assert.That(obj.EpochObject, Is.EqualTo(epochObject));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(epochObject.Kind));
                Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(epochObject.ToUniversalTime()));
                Assert.That(obj.Epoch, Is.EqualTo(epochObject.ToString("O")));
            }
            else
            {
                Assert.That(obj.EpochObject, Is.EqualTo(DateTime.UnixEpoch));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(obj.EpochObject.Kind, Is.EqualTo(DateTime.UnixEpoch.Kind));
                Assert.That(obj.EpochObject.ToUniversalTime(), Is.EqualTo(DateTime.UnixEpoch.ToUniversalTime()));
                Assert.That(obj.Epoch, Is.EqualTo(DateTime.UnixEpoch.ToString("O")));
            }
        }
    }
}
#endif
