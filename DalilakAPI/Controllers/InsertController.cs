using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DalilakAPI.Classes;
using DalilakAPI.Models;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Web;

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
        public string insertPlace(string name, string location, string description, string place_type,  string cityName)
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
                return place_id;
            }
            catch (Exception err)
            {
                Response.Redirect("http://api.dalilak.pro/System/Erro?error="+err.Message);
                return null;
            }
        }

        [HttpPost("UpdatePlace")]
        public bool UpdatePlace(string id, string name, string location, string des, string place_type, string CityName)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Places.Any(plc => plc.id == id))
                    {
                        var place = context.Places.Single(plc => plc.id == id);
                        if(name != null)
                            place.name = name;
                        if(location != null)
                            place.location = location;
                        if(des != null)
                            place.description = des;
                        if(place_type != null)
                            place.place_type = place_type;
                        if (CityName != null)
                        {
                            if (context.Cities.Any(item => item.name == CityName))
                            {
                                string Cityid = context.Cities.Single(item => item.name == CityName).id;
                                place.city_id = Cityid;
                            }
                        }
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
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
        public bool InsertImage(string place_id, string[] base64String )
        {
            // img example:
            //*byte[] image = System.IO.File.ReadAllBytes("Assets/Images/maxresdefault.jpg");
            //img = "data:image/jpg;base64," + base64String;

            try
            {
                using (var context = new Database())
                {
                    if (context.Places.Any(place => place.id == place_id))
                    {
                        var doc = context.Places.Single(place => place.id == place_id).related_doc;
                        _noSqlDatabase.AddImage(doc, base64String[0]); 
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
                    if (context.Users.Any(user => user.email == email || user.phone_num == phone))
                        return false;
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

        [HttpPost("UpdateProfile_")]
        public bool UpdateProfile(string user_id, byte[] img)
        {
            try
            {
                using (var context = new Database())
                {
                    if (context.Users.Any(user => user.id == user_id))
                    {
                        context.Users.Single(user => user.id == user_id).image = img;
                        context.SaveChanges();
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


        /* Insert Image for City */
        [HttpPost("Ads_")]
        public bool InsertAds(string admin_id, string city_id, byte[] image)
        {
            try
            {
                using (var context = new Database())
                {
                    var ad = new Ad()
                    {
                        id = default(int),
                        admin_id = admin_id,
                        city_id = city_id,
                        ad_image = image
                    };
                    context.Add(ad);
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

        /* Insert New Request */
        [HttpPost("Request_")]
        public bool InsertRequest(string user_id, byte[] file)
        {
            try
            {
                //byte[] file = System.IO.File.ReadAllBytes("Assets/PDF/Test.pdf");
                using (var context = new Database())
                {
                    var req = new Request()
                    {
                        id = default(int), 
                        admin_id="",
                        user_id=user_id,
                        file=file,
                        req_status=0
                    };
                    context.Add(req);
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

        [HttpPost("UpdateRequest_")]
        public bool UpdateRequest(string[] content)
        {
                // Content = new string[] {id, adminEmail, user_id, status}
            try
            {
                int reqId = int.Parse(content[0]);
                string email = content[1];
                string user_id = content[2];
                string status = content[3];

                using (var context = new Database())
                {
                    if(context.Requests.Any(req => req.id == reqId))
                    {
                        var req = context.Requests.Single(req => req.id == reqId && req.user_id == user_id);
                        var admin_id = context.Admin.Single(admn => admn.email == email).id;
                        if (status == "Accept")
                        {
                            req.req_status = 1;
                            req.admin_id = admin_id;
                            context.Users.Single(usr => usr.id == user_id).user_type = true;
                        }
                        else if(status == "Reject")
                        {
                            req.req_status = -1;
                            req.admin_id = admin_id;
                        }
                        context.SaveChanges();

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

        /* Insert Modification on the system by guiders */
        [HttpPost("Modification_")]
        public bool InsertModify(string user_id, string[] content, string operation)
        {
            try
            {
                // Content will be string has length equals to 1
                // when add place string = "name|location|description|placeType|CityName|Image"
                // when clinets read it, this letter '|' will be the splitter to make an array
                byte[] data = System.Text.Encoding.UTF8.GetBytes(content[0]);
                using (var context = new Database())
                {
                    if(context.Users.Any(user => user.id == user_id))
                    {
                        var modfy = new Modification()
                        {
                            id = default(int),
                            user_id = user_id,
                            admin_id = "",
                            file = data,
                            operation = operation
                        };
                        context.Add(modfy);
                        context.SaveChanges();
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

        [HttpPost("UpdateGuiderModify_")]
        public bool UpdateModify(string[] content)
        {
            try
            {
                int modiId = int.Parse(content[0]);
                string email = content[1];
                string user_id = content[2];
                string status = content[3];

                using (var context = new Database())
                {
                    if (context.Modifications.Any(modi => modi.id == modiId))
                    {
                        var modi = context.Modifications.Single(req => req.id == modiId && req.user_id == user_id);
                        var admin_id = context.Admin.Single(admn => admn.email == email).id;

                        modi.operation = status+"-"+modi.operation.Split('-')[1];
                        modi.admin_id = admin_id;
                        context.Users.Single(usr => usr.id == user_id).user_type = true;
                        context.SaveChanges();

                        return true;
                    }
                }
                return false;
            }
            catch (Exception err)
            {
                return false;
            }
        }

    }
}
