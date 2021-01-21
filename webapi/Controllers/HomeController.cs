using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace webapi 
{
    [Route("api/[Controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Get values from Api
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "ok" };
        }
    }
}