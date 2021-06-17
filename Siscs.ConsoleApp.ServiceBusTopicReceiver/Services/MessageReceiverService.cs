using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using Siscs.ConsoleApp.ServiceBusTopicReceiver.Models;

namespace Siscs.ConsoleApp.ServiceBusTopicReceiver.Services
{
    public class MessageReceiverService
    {
        private readonly ServiceBusConfig _serviceBusConfig;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ILogger<MessageReceiverService> _logger;

        private readonly Parametros _parametros;
        public MessageReceiverService(ILogger<MessageReceiverService> logger, 
                                    IOptions<ServiceBusConfig> serviceBusConfig,
                                    Parametros parametros)
        {
            _logger = logger;
            _serviceBusConfig = serviceBusConfig.Value;
            _parametros = parametros;
            _subscriptionClient = new SubscriptionClient(_serviceBusConfig.TopicConnection, 
                                                         _serviceBusConfig.TopicName, 
                                                         _parametros.SubscriptionName);

            _logger.LogInformation($"Topico = {_serviceBusConfig.TopicName}");
            _logger.LogInformation($"Subscription = {_parametros.SubscriptionName}");
        }

        public void RegisterMessageHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(ProcessarMensagensHandler, messageHandlerOptions);

        }

        private async Task ProcessarMensagensHandler(Message message, CancellationToken cancellationToken)
        {
            var messageString = Encoding.UTF8.GetString(message.Body);
            _logger.LogInformation($"*************************************");
            _logger.LogInformation($"[Mensagem Recebida de {_parametros.SubscriptionName}] ==> " + messageString);
            _logger.LogInformation($"*************************************");

            var pessoa = JsonConvert.DeserializeObject<Pessoa>(messageString);

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
  
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError($"Message handler - Tratamento - Exception: {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError("Ocorreu a exception:");
            _logger.LogError($"- Endpoint: {context.Endpoint}");
            _logger.LogError($"- Entity Path: {context.EntityPath}");
            _logger.LogError($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }

    }
}