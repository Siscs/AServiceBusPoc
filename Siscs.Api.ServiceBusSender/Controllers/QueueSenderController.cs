using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Siscs.Api.ServiceBusSender.Models;

namespace Siscs.Api.ServiceBusSender.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QueueSenderController : ControllerBase
    {
        private readonly QueueClient _queueClient;
        private readonly ServiceBusConfig _serviceBusConfig;
        public QueueSenderController(IOptions<ServiceBusConfig> serviceBusConfig)
        {
            _serviceBusConfig = serviceBusConfig.Value;
            _queueClient = new QueueClient(_serviceBusConfig.QueueConnection, 
                                           _serviceBusConfig.QueueName);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pessoa pessoa)
        {
            var pessoaJson = Newtonsoft.Json.JsonConvert.SerializeObject(pessoa);
            var messageInBytes = Encoding.UTF8.GetBytes(pessoaJson);
            
            var message = new Message(messageInBytes);
            message.ContentType = "application/json";

            await _queueClient.SendAsync(message);

            return Ok();
            
        }


    }
}