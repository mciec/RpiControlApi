using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RpiControlApi.Domain.Entities;

namespace RpiControlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServoController : ControllerBase
    {
        private readonly ILogger<ServoController> _logger;
        private readonly Sonar _sonar;

        public ServoController(ILogger<ServoController> logger, Sonar sonar)
        {
            _logger = logger;
            _sonar = sonar;
        }

        [HttpGet]
        public double GetDistance()
        {
            return _sonar.Distance;
        }

        
    }
}
