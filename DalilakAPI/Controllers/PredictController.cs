using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly ILogger<PredictController> _logger;
        public PredictController(ILogger<PredictController> logger)
        {
            _logger = logger;
        }
    }
}
