using System;
using Xunit;
using WritRead.Extensions;

namespace UnitTests
{
    public class GetReadableTimeSpanExtension
    {
        [Fact]
        public void ShouldHaveSameMessageForGreaterThan10Min()
        {
            int minutes = new Random().Next(10, 2000);
            TimeSpan ts = TimeSpan.FromMinutes(minutes);

            Assert.Equal("há mais de 10 minutos", ts.GetReadable());
        }

        [Fact]
        public void ShouldHaveDefaultMessageForZeroTimeSpan()
        {
            Assert.Equal("agora mesmo", TimeSpan.Zero.GetReadable());
        }

        [Fact]
        public void ShouldHaveSingularMessageFor1Second()
        {
            Assert.Equal("há 1 segundo", TimeSpan.FromSeconds(1).GetReadable());
        }

        [Fact]
        public void ShouldHavePluralMessageForOver1Second()
        {
            Assert.Equal("há 2 segundos", TimeSpan.FromSeconds(2).GetReadable());
        }

        [Fact]
        public void ShouldDisplaySingularMinutesForOneMinuteAndZeroSeconds()
        {
            TimeSpan ts = TimeSpan.FromMinutes(1);
            Assert.Equal("há 1 minuto", ts.GetReadable());
        }

        [Fact]
        public void ShouldDisplaySingularMinutesForOneMinuteAndOneSecond()
        {
            TimeSpan ts = TimeSpan.FromSeconds(61);
            Assert.Equal("há 1 minuto e 1 segundo", ts.GetReadable());
        }

        [Fact]
        public void ShouldDisplaySingularMinutesForOneMinuteAndPluralSecondsForTwoSeconds()
        {
            TimeSpan ts = TimeSpan.FromSeconds(62);
            Assert.Equal("há 1 minuto e 2 segundos", ts.GetReadable());
        }

        [Fact]
        public void ShouldDisplayPluralMinutesForOverOneMinuteAndZeroSeconds()
        {
            TimeSpan ts = TimeSpan.FromMinutes(2);
            Assert.Equal("há 2 minutos", ts.GetReadable());
        }

        [Fact]
        public void ShouldDisplayPluralMinutesForOverOneMinuteAndOneSecond()
        {
            TimeSpan ts = TimeSpan.FromSeconds(121);
            Assert.Equal("há 2 minutos e 1 segundo", ts.GetReadable());
        }

        [Fact]
        public void ShouldDisplayPluralMinutesForOverOneMinuteAndPluralSecondsForTwoSeconds()
        {
            TimeSpan ts = TimeSpan.FromSeconds(122);
            Assert.Equal("há 2 minutos e 2 segundos", ts.GetReadable());
        }
    }
}
