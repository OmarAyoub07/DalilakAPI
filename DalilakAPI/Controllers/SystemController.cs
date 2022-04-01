using DalilakAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

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

        [HttpGet("RNG_")]
        public string RNG()
        {
            Random r = new Random();

            return r.Next(0,99999).ToString("D5");
        }
    }
}
