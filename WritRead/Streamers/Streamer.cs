using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace WritRead.Streamers
{
    public abstract class Streamer
    {
        protected readonly string MicroserviceName;
        protected readonly string TopicName;
        private Thread _thread;
        protected AutoResetEvent _waitHandle = new AutoResetEvent(false);
        protected readonly ILogger<Streamer> _logger;

        public bool ShouldContinue { get; private set; }

        public Streamer(string microserviceName, string topicName, ILoggerFactory logger)
        {
            if (string.IsNullOrWhiteSpace(microserviceName))
                throw new ArgumentNullException(nameof(microserviceName));

            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentNullException(nameof(topicName));

            if (logger == null)
                throw new ArgumentNullException();

            MicroserviceName = microserviceName;
            TopicName = topicName;
            _logger = logger.CreateLogger<Streamer>();
        }

        public virtual void Start()
        {
            if (ShouldContinue)
            {
                _logger.LogError("O Streamer atual já está iniciado");
                throw new InvalidOperationException("Já iniciado");
            }

            ShouldContinue = true;

            _thread = new Thread(() =>
            {
                while (ShouldContinue)
                {
                    Run();
                }

                CleanUp();
            });

            _thread.Start();
        }

        public virtual void Stop()
        {
            ShouldContinue = false;
        }

        public virtual void Join()
        {
            if (_thread == null)
            {
                _logger.LogError("O Streamer atual não está iniciado e, portanto, não pode ser dado join");
                throw new InvalidOperationException("Não iniciado");
            }

            _thread.Join();
        }

        protected abstract void Run();
        protected abstract void CleanUp();
    }
}