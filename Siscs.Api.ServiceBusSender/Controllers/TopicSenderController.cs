using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Siscs.Api.ServiceBusSender.Models;

namespace Siscs.Api.ServiceBusSender.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TopicSenderController : ControllerBase
    {
        private readonly ServiceBusConfig _serviceBusConfig;
        private readonly TopicClient _topicClient;
        public TopicSenderController(IOptions<ServiceBusConfig> serviceBusConfig)
        {
            _serviceBusConfig = serviceBusConfig.Value;
            _topicClient = new TopicClient(_serviceBusConfig.TopicConnection, 
                                           _serviceBusConfig.TopicName);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pessoa pessoa)
        {
            var messageString = JsonConvert.SerializeObject(pessoa);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var message = new Message(messageBytes);
            message.ContentType = "application/json";

            await _topicClient.SendAsync(new Message(messageBytes));

            return Ok();
        }
        
    }
}