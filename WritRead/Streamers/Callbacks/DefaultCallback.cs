using System;
using Microsoft.Extensions.Logging;
using WritRead.Extensions;
using WritRead.Model;

namespace WritRead.Streamers.Callbacks
{
    public class DefaultCallback : ICallback
    {
        private readonly ILogger<DefaultCallback> _logger;

        public DefaultCallback(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<DefaultCallback>();
        }

        public void ProcessReceivedMessage(MessageModel messageModel)
        {
            var tempoEnvio = DateTime.UtcNow - messageModel.Timestamp;
            _logger.LogInformation("=====================================================");
            _logger.LogInformation("Recebi a seguinte mensagem");
            _logger.LogInformation($"Conteúdo: {messageModel.Text}");
            _logger.LogInformation($"Enviado por: {messageModel.ID} {tempoEnvio.GetReadable()}");
            _logger.LogInformation($"com o identificador único {messageModel.ReqID}");
        }
    }
}