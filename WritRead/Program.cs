using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using WritRead.Extensions;
using WritRead.Model;
using WritRead.Streamers;
using WritRead.Streamers.Callbacks;
using static WritRead.Streamers.Consumer;

namespace WritRead
{
    class Program
    {
        private static ILogger<Program> _logger;

        static void Main(string[] args)
        {
            var serviceProvider = LoadConfig(args);
            DateTimeOffset inicioExecucao = DateTimeOffset.UtcNow;
            
            try
            {
                var consumer = serviceProvider.GetService<Consumer>();
                var producer = serviceProvider.GetService<Producer>();

                producer.Start();
                consumer.Start();

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    _logger.LogInformation("Solicitação para fechar o programa recebida. Enviando sinal de parada");

                    producer.Stop();
                    consumer.Stop();
                };

                consumer.Join();
                producer.Join();
            }
            catch (Exception err)
            {
                _logger.LogError($"Erro na aplicação: {err.Message}");
            }
            finally
            {
                _logger.LogInformation("Finalizando programa");      

                var fimExecucao = DateTimeOffset.UtcNow;
                _logger.LogInformation("☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠");
                _logger.LogInformation($"Aqui jaz {args[0]}");
                _logger.LogInformation($"☦ {inicioExecucao.ToLocalTime().ToString("dd/MM/yyyy - HH:mm:ss")}");
                _logger.LogInformation($"☦ {fimExecucao.ToLocalTime().ToString("dd/MM/yyyy - HH:mm:ss")}");
                _logger.LogInformation($"Iniciado {(fimExecucao - inicioExecucao).GetReadable()}");
                _logger.LogInformation("☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠☠");
            }
        }

        static IServiceProvider LoadConfig(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception($"Eu preciso que você me passe três parâmetros. {Environment.NewLine}O nome desse microsserviço, o nome do tópico para eu inserir informações, o nome do tópico para eu ler informações. {Environment.NewLine}Por exemplo, 'WritRead ESPIRIQUIDIBERTO PERGAMINHO GRIMOIRE' fará com que eu escreva no tópico PERGAMINHO e leia do tópico GRIMOIRE sob o nome de ESPIRIQUIDIBERTO");
            }
           

            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();
            
            var configuration = builder.Build();

            var serviceProvider = new ServiceCollection()                        
                        .AddOptions()
                        .Configure<Configs>(config =>
                        {
                            configuration.GetSection("Config").Bind(config);
                            config.MicroserviceName = args[0];
                            config.ConsumerTopicName = args[1];
                            config.ProducerTopicName = args[2];
                        })
                        .AddSingleton<Consumer>()
                        .AddSingleton<Producer>()
                        .AddSingleton<ICallback, DefaultCallback>()
                        .AddSingleton<IProducer<Null, string>>(sp =>
                        {
                            var configs = sp.GetService<IOptions<Configs>>();
                            var producerConfig = new ProducerConfig { BootstrapServers = configs.Value.BootstrapServers };
                            return new ProducerBuilder<Null, string>(producerConfig).Build();
                        })
                        .AddSingleton<IConsumer<Ignore, string>>(sp =>
                        {
                            var configs = sp.GetService<IOptions<Configs>>();
                            var consumerConfig = new ConsumerConfig
                            {
                                GroupId = configs.Value.GroupId,
                                BootstrapServers = configs.Value.BootstrapServers,
                                AutoOffsetReset = AutoOffsetReset.Earliest
                            };

                            return new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

                        })                        
                        .AddLogging(conf => conf.AddSerilog())
                        .BuildServiceProvider();

            var configs = serviceProvider.GetService<IOptions<Configs>>();
            string logFileName = $"{configs.Value.MicroserviceName}-{DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture)}.txt";
            string saveLogPath = Path.Combine("logs", logFileName);
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.Console()
               .WriteTo.File(saveLogPath)
               .CreateLogger();

            _logger = serviceProvider.GetService<ILoggerFactory>()                                        
                                        .CreateLogger<Program>();

            _logger.LogInformation("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");
            _logger.LogInformation("Prazer em conhecê-lo(a), serei seu log que vai guiá-lo durante a execução deste programa ;)");
            _logger.LogInformation($"A propósito, tudo que você ler neste console está disponível no arquivo {saveLogPath}");
            _logger.LogInformation("Para finalizar esse programa, pressione CONTROL+C no terminal apenas uma vez e aguarde que o programa será finalizado graciosamente");
            _logger.LogInformation("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-");

            return serviceProvider;
        }
    }
}
