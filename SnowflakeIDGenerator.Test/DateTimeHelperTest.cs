namespace SnowflakeID.Test
{
    public class DateTimeHelperTest
    {
        [Test]
        public void TimestampFromEpochTest()
        {
            DateTime d = DateTime.UtcNow;
            ulong oldWay = ((ulong)d.Subtract(UnixEpoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            ulong newWay = DateTimeHelper.TimestampMillisFromEpoch(d, UnixEpoch);
            Assert.That(newWay, Is.EqualTo(oldWay));
        }

        [Test]
        public void DriftTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(DateTimeHelper.TimestampMillisFromEpoch(UnixEpoch, UnixEpoch), Is.EqualTo(0));
                Assert.That(DateTimeHelper.TimestampMillisFromEpoch(CustomEpoch, UnixEpoch), Is.EqualTo(1577836800000));
            });
        }



        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime CustomEpoch = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
