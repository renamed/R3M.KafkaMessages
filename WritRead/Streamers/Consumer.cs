using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using WritRead.Model;
using WritRead.Streamers.Callbacks;

namespace WritRead.Streamers
{
    public class Consumer : Streamer
    {
        private readonly ICallback _processMessage;
        private IConsumer<Ignore, string> _consumer;
        private readonly Configs _config;
        private CancellationTokenSource _cancellationTokenSource;

        public Consumer(IOptions<Configs> config, ICallback processMessage, ILoggerFactory logger, IConsumer<Ignore, string> consumer) : base(config.Value.MicroserviceName, config.Value.ConsumerTopicName, logger)
        {
            _config = config.Value;
            _processMessage = processMessage;
            _consumer = consumer;
        }

        protected override void Run()
        {

            _consumer.Subscribe(TopicName);
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                _logger.LogDebug("Consumidor vai esperar chegar uma mensagem. Até mais!");
                var cr = _consumer.Consume(_cancellationTokenSource.Token);                
                _processMessage.ProcessReceivedMessage(JsonSerializer.Deserialize<MessageModel>(cr.Message.Value));

                _logger.LogDebug("Consumidor recebeu e encaminhou uma mensagem.");
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Consumidor encontrou problemas: {e.Message}");                
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Solicitação de cancelamento recebida pelo Consumidor");
                _consumer.Close();
                _logger.LogInformation("Consumidor finalizado");
            }
        }

        protected override void CleanUp()
        {            
            _consumer.Dispose();
            _consumer = null;
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();
            base.Stop();
        }
    }
}