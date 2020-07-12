using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.classes.Helpers;
using WritRead.Streamers;
using Xunit;

namespace UnitTests.classes
{
    public class StreamerTest
    {
        private const string VALID_SERVICE_NAME = "VALID_SERVICE_NAME";
        private const string VALID_TOPIC_NAME = "VALID_TOPIC_NAME";
        private readonly ILoggerFactory VALID_LOGGER_FACTORY = new NullLoggerFactory();


        [Fact]
        public void ShouldCallRunWhenStarted()
        {
            var dumbStreamer = new DumbStreamer(VALID_SERVICE_NAME, VALID_TOPIC_NAME, VALID_LOGGER_FACTORY);

            dumbStreamer.RunCalls = 0;
            dumbStreamer.Start();
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            dumbStreamer.Stop();            
            Thread.Sleep(TimeSpan.FromMilliseconds(500));


            Assert.True(dumbStreamer.RunCalls > 0);
        }

        [Fact]
        public void ShouldCallCleanUpWhenStopped()
        {
            var dumbStreamer = new DumbStreamer(VALID_SERVICE_NAME, VALID_TOPIC_NAME, VALID_LOGGER_FACTORY);

            dumbStreamer.CleanUpCalls = 0;
            dumbStreamer.Start();
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            dumbStreamer.Stop();            
            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            Assert.True(dumbStreamer.CleanUpCalls > 0);
        }

        [Fact]
        public void ShouldNotAllowToStartTwice() 
        {
            var dumbStreamer = new DumbStreamer(VALID_SERVICE_NAME, VALID_TOPIC_NAME, VALID_LOGGER_FACTORY);

            dumbStreamer.Start();
            var exception = Assert.Throws<InvalidOperationException>(dumbStreamer.Start);
            Assert.Equal("Já iniciado", exception.Message);
        }

        [Fact]
        public void ShouldNotAllowToJoinWithNoStart()
        {
            var dumbStreamer = new DumbStreamer(VALID_SERVICE_NAME, VALID_TOPIC_NAME, VALID_LOGGER_FACTORY);

            var exception = Assert.Throws<InvalidOperationException>(() => dumbStreamer.Join());
            Assert.Equal("Não iniciado", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]        
        public void ShouldNotAllowInvalidServiceName(string serviceName)
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new DumbStreamer(serviceName, VALID_TOPIC_NAME, VALID_LOGGER_FACTORY));            
            Assert.Contains("Value cannot be null", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ShouldNotAllowInvalidTopicName(string topicName)
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new DumbStreamer(VALID_SERVICE_NAME, topicName, VALID_LOGGER_FACTORY));            
            Assert.Contains("Value cannot be null", exception.Message);
        }
    }
}