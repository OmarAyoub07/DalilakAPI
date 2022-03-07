using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DalilakAPI.Classes;
using DalilakAPI.Models;
using System.Linq;
using System;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DropController : ControllerBase
    {
        private readonly ILogger<DropController> _logger;
        private NoSqlDatabase _noSqlDatabase = new NoSqlDatabase();

        public DropController(ILogger<DropController> logger)
        {
            _logger = logger;
        }

        // Delete one place from database with deleting attached JSON Documents from RavenDB 
        [HttpPost("Place_")]
        public bool dropPlace(string id)
        {
            try
            {
                using (var context = new Database())
                {
                    bool isExist = context.Places.Any(item => item.id == id);
                    if (isExist)
                    {
                        var item = context.Places.Single(item => item.id == id);
                        _noSqlDatabase.deleteDoc(item.statstc_doc);
                        _noSqlDatabase.deleteDoc(item.related_doc);
                        context.Remove(item);
                        context.SaveChanges();
                    }
                    return isExist;
                }
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }

        [HttpPost("User_")]
        public bool DropUser(string id)
        {
            try
            {
                using (var context = new Database())
                {
                    if (context.Users.Any(user => user.id == id))
                    {
                        var user = context.Users.Single(user => user.id == id);
                        _noSqlDatabase.deleteDoc(user.record_doc);
                        context.Remove(user);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
