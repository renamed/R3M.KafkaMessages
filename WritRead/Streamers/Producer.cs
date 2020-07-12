

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using WritRead.Model;

namespace WritRead.Streamers
{
    public class Producer : Streamer
    {
        private IProducer<Null, string> _producer;
        private readonly Configs _config;
        

        public Producer(IOptions<Configs> config, IProducer<Null, string> producer, ILoggerFactory logger) : base(config.Value.MicroserviceName, config.Value.ProducerTopicName, logger)
        {
            _config = config.Value;
            _producer = producer;            
        }

        protected override void Run()
        {
            try
            {
                var message = new MessageModel
                {
                    Text = _config.Text,
                    ID = MicroserviceName,
                    Timestamp = DateTime.UtcNow,
                    ReqID = Guid.NewGuid().ToString("N")
                };

                _logger.LogDebug($"Produtor vai enviar a mensagem de ID {message.ReqID}");

                var messageObj = new Message<Null, string> { Value = JsonSerializer.Serialize(message) };                
                _producer.Produce(TopicName, messageObj);
                
                _logger.LogDebug($"Produtor enviou a mensagem");

                _logger.LogDebug($"Produtor vai dormir");
                _waitHandle.Set();                
                Thread.Sleep(TimeSpan.FromSeconds(_config.IntervalBetweenMessagesInSeconds));
                _logger.LogDebug($"Produtor acordou");
            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError($"Produtor encontrou problemas: {e.Message}");
            }
        }

        protected override void CleanUp()
        {
            _logger.LogInformation("Solicitação de cancelamento recebida pelo Produtor");
            _producer.Dispose();
            _producer = null;
            _logger.LogInformation("Produtor finalizado");
        }
    }
}
