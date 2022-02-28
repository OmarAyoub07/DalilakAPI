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
    public class InsertController : ControllerBase
    {
        private readonly ILogger<InsertController> _logger;
        private NoSqlDatabase _noSqlDatabase = new NoSqlDatabase();

        public InsertController(ILogger<InsertController> logger)
        {
            _logger = logger;
        }

        // Insert New Place to database with the JSON Documnets into RavenDB - Setup all setting for one place.
        [HttpPost("Place_")]
        public bool insertPlace(string name, string location, string description, string place_type,  string cityName)
        {
            try
            {
                string place_id = Guid.NewGuid().ToString();
                var related_Docs = _noSqlDatabase.createNewDoc_forPlace(place_id);

                using (var context = new Classes.Database())
                {
                    var id = context.Cities.Where(item => item.name == cityName).Select(item => item.id).Single();
                    var place = new Place
                    {
                        id = place_id,
                        name = name,
                        location = location,
                        description = description,
                        place_type = place_type,                       
                        related_doc = related_Docs[1],
                        statstc_doc = related_Docs[0],
                        city_id = id
                    };
                    context.Add(place);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }
        
        [HttpPost("User_")]
        public bool insertUser(string name , string phone , string email)
        {
            try
            {

                string userID = Guid.NewGuid().ToString("N");
                string Jsondoc = _noSqlDatabase.createNewDocforUser(userID);


                using (var context=new Database())
                {
                    var user = new User
                    {

                        email = email,
                        name = name,
                        phone_num = phone,
                        id = userID,
                        record_doc = Jsondoc


                    };
                    context.Add(user);
                    context.SaveChanges();
                }
                return true;

                

               

            }

            catch(Exception ex)
            {
                return false;

            }

        }
    }
}
