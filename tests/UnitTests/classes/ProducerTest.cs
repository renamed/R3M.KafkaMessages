using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using WritRead.Model;
using WritRead.Streamers;
using Xunit;

namespace UnitTests.classes
{
    public class ProducerTest
    {
        private readonly ILoggerFactory VALID_LOGGER_FACTORY = new NullLoggerFactory();
        private const string VALID_MESSAGE = "Hi";
        private const string VALID_SERVICE_NAME = "VALID_SERVICE_NAME";
        private const string VALID_TOPIC_NAME = "VALID_TOPIC_NAME";

        private Producer GetProducerWithMocks(Exception exceptionOnConsume)
        {            
            var options = Options.Create<Configs>(new Configs {
                ConsumerTopicName = VALID_TOPIC_NAME,
                ProducerTopicName = VALID_TOPIC_NAME,
                MicroserviceName = VALID_SERVICE_NAME
            });
            var substituteProducer = Substitute.For<IProducer<Null, string>>();

            if (exceptionOnConsume != null)
            {
                substituteProducer
                    .When(x => x.Produce(Arg.Any<string>(), Arg.Any<Message<Null, string>>(), null))
                    .Do(x => throw exceptionOnConsume);
            }

            return new Producer(options, substituteProducer, VALID_LOGGER_FACTORY);
        }

        [Fact]
        public void ShouldCompleteWithNoErrors()
        {
            var producer = GetProducerWithMocks(null);

            producer.Start();
            Thread.Sleep(500);
            producer.Stop();
        }

        [Fact]
        public void ShouldNotHaltOnProduceException()
        {
            var exception = new ProduceException<Null, string>(new Error(ErrorCode.BrokerNotAvailable), new DeliveryResult<Null, string>());
            var producer = GetProducerWithMocks(exception);

            producer.Start();
            Thread.Sleep(500);
            producer.Stop();
        }
    }
}