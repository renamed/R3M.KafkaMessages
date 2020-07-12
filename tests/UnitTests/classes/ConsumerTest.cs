using System.Threading;
using System.Linq;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using WritRead.Model;
using WritRead.Streamers;
using Xunit;
using System;
using System.Text.Json;
using WritRead.Streamers.Callbacks;

namespace UnitTests.classes
{
    public class ConsumerTest
    {
        private readonly ILoggerFactory VALID_LOGGER_FACTORY = new NullLoggerFactory();
        private const string VALID_MESSAGE = "Hi";
        private const string VALID_SERVICE_NAME = "VALID_SERVICE_NAME";
        private const string VALID_TOPIC_NAME = "VALID_TOPIC_NAME";

        private Consumer GetConsumerWithMocks(Exception exceptionOnConsume)
        {
            var options = Options.Create<Configs>(new Configs
            {
                ConsumerTopicName = VALID_TOPIC_NAME,
                ProducerTopicName = VALID_TOPIC_NAME,
                MicroserviceName = VALID_SERVICE_NAME
            });

            var message = new MessageModel
            {
                Text = VALID_MESSAGE
            };

            var substituteConsumer = Substitute.For<IConsumer<Ignore, string>>();
            substituteConsumer.Subscribe(Arg.Any<string>());

            if (exceptionOnConsume == default)
            {
                substituteConsumer.Consume(Arg.Any<CancellationToken>()).Returns(x =>
                {
                    return new ConsumeResult<Ignore, string>
                    {
                        Message = new Message<Ignore, string> { Value = JsonSerializer.Serialize(message) }
                    };
                });
            }
            else
            {
                substituteConsumer
                .When(x => x.Consume(Arg.Any<CancellationToken>()))
                .Do(x => throw exceptionOnConsume);
            }

            var callback = Substitute.For<ICallback>();
            callback
                .When(x => x.ProcessReceivedMessage(Arg.Any<MessageModel>()))
                .Do(x => Assert.Equal(VALID_MESSAGE, (x.Args()[0] as MessageModel).Text));

            return new Consumer(options, callback, VALID_LOGGER_FACTORY, substituteConsumer);
        }

        [Fact]
        public void ShouldCompleteWithNoErrors()
        {
            var consumer = GetConsumerWithMocks(default);

            consumer.Start();
            Thread.Sleep(500);
            consumer.Stop();
        }

        [Fact]
        public void ShouldNotHaltOnConsumerException()
        {
            
            var consumer = GetConsumerWithMocks(new ConsumeException(new ConsumeResult<byte[], byte[]>(), new Error(ErrorCode.BrokerNotAvailable)));

            consumer.Start();
            Thread.Sleep(500);
            consumer.Stop();
        }

        [Fact]
        public void ShouldNotHaltOnCanceling()
        {            
            var consumer = GetConsumerWithMocks(new OperationCanceledException());

            consumer.Start();
            Thread.Sleep(500);
            consumer.Stop();
        }
    }
}