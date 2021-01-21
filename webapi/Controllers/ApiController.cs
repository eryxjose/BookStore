using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using webapi.Contracts;

namespace webapi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        
        private readonly ILoggerService _logger;

        public ApiController(ILoggerService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get values from Api
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Request para Get() em ApiController.");
            return new string[] { "ok" };
        }
    }
}