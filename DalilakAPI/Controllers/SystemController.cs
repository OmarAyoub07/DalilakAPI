using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        public SystemController(ILogger<SystemController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Erro")]
        public string systemError(string error)
        {
            return error;
        }
    }
}
