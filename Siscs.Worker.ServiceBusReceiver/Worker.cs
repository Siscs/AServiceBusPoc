using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Siscs.Worker.ServiceBusReceiver
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly QueueClient _queueClient;
        private readonly IConfiguration _configuration;
        private readonly string _queueName;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var azureServiceBusConnection = _configuration.GetSection("ServiceBus:AzureServiceBusConnection").Value;
            _queueName =  _configuration.GetSection("ServiceBus:QueueName").Value;
            _queueClient = new QueueClient(azureServiceBusConnection, _queueName, ReceiveMode.PeekLock);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando Recebimento de mensagemem: {time}", DateTimeOffset.Now);

                await Task.Run(() => {

                    var messageHandlerOptions = new MessageHandlerOptions( 
                    async (e) => {
                        await ErroHandler(e);
                    });
                    // {
                    //     AutoComplete = false
                    // };

                    _queueClient.RegisterMessageHandler(ProcessMessageHandler, messageHandlerOptions);

                });
                
            }
        }

        private async Task ProcessMessageHandler(Message message, CancellationToken cancellationToken)
        {
            var messageReceived = Encoding.UTF8.GetString(message.Body);
            var pessoaSchema = new {Nome = "", SobreNome = "", Idade=0 };
            var pessoa = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(messageReceived, pessoaSchema);
            _logger.LogInformation("[Mensagem Recebida:] " + messageReceived );
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            // await Task.CompletedTask;
        }

        private Task ErroHandler(ExceptionReceivedEventArgs e)
        {
            _logger.LogError("[Falha] " +
                e.Exception.GetType().FullName + " " +
                e.Exception.Message);
            
            return Task.CompletedTask;
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _queueClient.CloseAsync();
            _logger.LogInformation("Conexão fechada com o azure!");
            await base.StopAsync(cancellationToken);
        }
    }
}
