using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using DalilakAPI.Classes;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger; // add the class as controller
        }

        // This function require a post from client side to compare the posted data with database content.
        [HttpPost("admin_")] // Routing of the URL will be ".../Login/admin_?email=any@email&pass=anyPassword"
                            // Where '/Login/' represent the controller
        public bool loginToWeb(string email, string pass)
        {
            try
            {
                using (var context = new Database()) // one time connection with database context
                {
                    // if there any admin where email = 'email' and password = 'pass'
                    bool admin = context.Admin.Any(item => item.email == email && item.password == pass);
                    return admin;
                }
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }


        [HttpPost("user_")] // routing
        public string loginToApp(string phone)
        {
            try
            {
                phone = "+966"+phone;
                using (var context = new Database()) // one time connection
                {
                    if (context.Users.Any(item => item.phone_num == phone)) // if true (isAny)
                    {
                        var user = context.Users.Single(item => item.phone_num == phone).id; // get user id
                        return user;
                    }
                    return "notExist";
                }
            }
            catch (Exception err)
            {
                return null;
            }
        }

    }
}
