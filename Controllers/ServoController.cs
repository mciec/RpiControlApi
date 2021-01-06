using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RpiControl.Domain.Entities;

namespace RpiControlApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ServoController : ControllerBase
    {
        private readonly ILogger<ServoController> _logger;
        private readonly CommunicationObject _communicationObject;

        public ServoController(ILogger<ServoController> logger, CommunicationObject communicationObject)
        {
            _logger = logger;
            _communicationObject = communicationObject;
        }

        [HttpGet]
        public double GetDistance()
        {
            return _communicationObject.Distance;
        }

        
    }
}
