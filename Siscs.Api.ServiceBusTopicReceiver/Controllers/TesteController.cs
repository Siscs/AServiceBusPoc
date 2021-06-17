using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Siscs.Api.ServiceBusTopicReceiver.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TesteController : ControllerBase
    {
        private readonly ILogger<TesteController> _logger;

        public TesteController(ILogger<TesteController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {

            return Ok(new {Ok="Ok"});
           
        }
    }
}
