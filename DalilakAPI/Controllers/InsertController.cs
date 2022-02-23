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
        public bool insertPlace(string name, string location, string description, string place_type, string crowd_status, int likes, int visits, string cityName)
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
                        crowd_status = crowd_status,
                        related_doc = related_Docs[1],
                        statstc_doc = related_Docs[0],
                        totl_likes = likes,
                        totl_visits = visits,
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

    }
}
