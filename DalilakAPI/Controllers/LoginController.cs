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
            _logger = logger;
        }

        // This function require a post from client side to compare the posted data with database content.
        [HttpPost("admin_")]
        public bool loginToWeb(string email, string pass)
        {
            try
            {
                using (var context = new Database())
                {
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

        [HttpPost("user_")]
        public bool loginToApp(string phone)
        {
            try
            {
                phone = "+966"+phone;
                using (var context = new Database())
                {
                    bool user = context.Users.Any(item => item.phone_num == phone);
                    return user;
                }
            }
            catch (Exception err)
            {
                return false;
            }
        }
        //Return user id to the client
        [HttpPost("getUser_")]
        public string getUser(String number)
        {

            try
            {
                using (var context = new Database()) {
                    string id = context.Users.Single(item => item.phone_num == "+966"+number).id;
                    return id;
                }
                   
                    
            }
            catch (Exception err)
            {
                return err.Message;

            }
        }
        //Return user id to client
        [HttpPost("getEmail_")]
        public string getEmail(String email)
        {

            try
            {
                using (var context = new Database())
                {
                    string id = context.Users.Single(item => item.email == email).id;
                    return id;
                }


            }
            catch (Exception err)
            {
                return err.Message;

            }
        }


    }
}
