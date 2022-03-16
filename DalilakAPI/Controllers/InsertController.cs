using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DalilakAPI.Classes;
using DalilakAPI.Models;
using System.Linq;
using System;
using System.IO;

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
                string place_id = Guid.NewGuid().ToString("N");
                var related_Docs = _noSqlDatabase.createNewDoc_forPlace(place_id);

                using (var context = new Database())
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

        // Comment in specific place
        [HttpPost("Comment_")]
        public bool InsertComment(string place_id, string user_id, string message)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Places.Any(plc => plc.id == place_id) && context.Users.Any(usr => usr.id == user_id))
                    {
                        var doc = context.Places.Single(plc => plc.id == place_id).related_doc;
                        _noSqlDatabase.AddComment(doc, user_id, message);

                        return true;
                    }
                }
                return false;
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }

        // Image to specific Plage
        [HttpPost("PlaceImage_")]
        public bool InsertImage(string place_id, string img)
        {
            // img example:
            //*byte[] image = System.IO.File.ReadAllBytes("Assets/Images/maxresdefault.jpg");
            //*string base64String = Convert.ToBase64String(image, 0, image.Length);
            //*img = "data:image/jpg;base64," + base64String;

            try
            {
                using (var context = new Database())
                {
                    if(context.Places.Any(place => place.id == place_id))
                    {
                        var doc = context.Places.Single(place => place.id == place_id).related_doc;
                        _noSqlDatabase.AddImage(doc, img);
                    }
                }
                    return true;
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }

        [HttpPost("Visit_")]
        public bool InsertVisit(string place_id, string date ,string time, int visitors_Num)
        {
            try
            {
                using (var context = new Database())
                {
                    if (context.Places.Any(place => place.id == place_id)) 
                    {
                        var doc_id = context.Places.Single(place => place.id == place_id).statstc_doc;
                        _noSqlDatabase.AddVisits(doc_id, date, time, visitors_Num);
                    }
                }
                    return true;
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }
        }

        // Insert Rate or  for a place from specific user
        [HttpPost("Rate_")]
        public bool InsertRate(string user_id, string place_id, int rate, bool favorit)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Users.Any(user => user.id == user_id) && context.Places.Any(place => place.id == place_id))
                    {
                        var doc = context.Users.Single(user => user.id == user_id).record_doc;
                        _noSqlDatabase.AddRate(doc, place_id, rate, favorit);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception err)
            {
                return false;
            }
        }


        
        /* function to insert data related to users */
        [HttpPost("NewUser_")]
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
                        phone_num = "+966"+phone,
                        id = userID,
                        record_doc = Jsondoc,
                        image = System.IO.File.ReadAllBytes("Assets/Images/traveler.png")
                };
                    context.Add(user);
                    context.SaveChanges();
                }
                return true;
            }
            catch(Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return false;
            }

        }

        [HttpPost("UpdateUser_")]
        public bool UpdateUser(string id, string email, string phone, string name, int age, bool userType, string info, string cityName)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Users.Any(user => user.id == id))
                    {
                        var user = context.Users.Single(user => user.id == id);

                        if(email != null)
                            user.email = email;

                        if(phone != null)
                            user.phone_num = "+966"+phone;

                        if(name != null)
                            user.name = name;

                        if(age != 0)
                            user.age = age;

                        if(userType != user.user_type)
                            user.user_type = userType;

                        if(info != null)
                            user.information = info;

                        if(cityName != null)
                        {
                            if (context.Cities.Any(item => item.name == cityName))
                            {
                                string Cityid = context.Cities.Single(item => item.name == cityName).id;
                                user.city_id = Cityid;
                            }                               
                        }

                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                return false ;
            }
        }


    }
}
