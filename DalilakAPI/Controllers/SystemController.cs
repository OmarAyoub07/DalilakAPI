using DalilakAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        private StoreImages storeImages = new StoreImages();

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

        [HttpPost("Packets_")] // User id who is doing post to an image
        public bool PacketsofImage(string user_id, string packet)
        {
            try
            {
                storeImages.SetPacket(user_id, packet);
                return true;
            }
            catch (Exception err)
            {
                return false;

            }
        }
    }
}
