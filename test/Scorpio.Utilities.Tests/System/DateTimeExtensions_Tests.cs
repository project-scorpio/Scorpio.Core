using System;

using Shouldly;

using Xunit;

namespace System
{
    public class DateTimeExtensions_Tests
    {
        [Fact]
        public void ToUnixTimestamp_ShouldReturnCorrectValue()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch.ToUnixTimestamp().ShouldBe(0L);
        }

        [Fact]
        public void ToUnixTimestamp_ShouldReturnPositiveValue_ForDateAfterEpoch()
        {
            var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date.ToUnixTimestamp().ShouldBeGreaterThan(0L);
        }

        [Fact]
        public void ToUnixTimestamp_ShouldReturnExpectedValue()
        {
            // 2024-01-01 00:00:00 UTC = 1704067200
            var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date.ToUnixTimestamp().ShouldBe(1704067200L);
        }
    }
}
