using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using DalilakAPI.Models;
using System.Linq;
using System;
using DalilakAPI.Classes;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> _logger;
        private NoSqlDatabase _noSqlDatabase = new NoSqlDatabase();

        public QueryController(ILogger<QueryController> logger)
        {
            _logger = logger;
        }

        /* Functions to return data related to users */

        // Return all user for admin
        [HttpGet("Users_")]
        public IEnumerable<User> GetUsers()
        {
            try
            {
                List<User> users = new List<User>();
                int i = 0;

                using (var context = new Database())
                {
                    foreach (var user in context.Users)
                    {
                        users.Add(user);
                        i++;
                    }
                }

                return Enumerable.Range(0, i).Select(index => new User
                {
                    id = users[index].id,
                    name = users[index].name,
                    email = users[index].email,
                    phone_num = users[index].phone_num,
                    user_type = users[index].user_type,
                    city_id = users[index].city_id,
                    age = users[index].age,
                    information = users[index].information,
                });
            }
            catch (Exception err)
            {
                return Enumerable.Empty<User>();
            }
        }

        // Get All rows from Admin Table at database or one admin
        [HttpGet("admin_")]
        public IEnumerable<Admin> GetAdmins(string admin_id)
        {
            try
            {
                if (admin_id != null)
                {
                    Admin admin = null;
                    using (var context = new Database())
                    {
                        admin = context.Admin.Single(item => item.id == admin_id);
                    }

                    return Enumerable.Range(0, 1).Select(index => new Admin
                    {
                        email = admin.email
                    });
                }


                List<Admin> admins = new List<Admin>();
                int i = 0;
                using (var context = new Classes.Database())
                {
                    foreach (var item in context.Admin)
                    {
                        i++;
                        admins.Add(item);
                    }
                }

                return Enumerable.Range(0, i).Select(index => new Models.Admin
                {
                    email = admins[index].email
                }).ToArray();
            }
            catch (Exception err)
            {
                return null;
            }
        }

        // Return One user (when any user login to the system, or if the admin query for user)
        [HttpGet("User_")]
        public IEnumerable<User> GetUser(string id)
        {
            try
            {
                var user = new User();

                using (var context = new Database())
                {
                    if (context.Users.Any(user => user.id == id))
                        user = context.Users.Single(user => user.id == id);
                }

                return Enumerable.Range(0, 1).Select(index => new User
                {
                    id = user.id,
                    name = user.name,
                    email = user.email,
                    phone_num = user.phone_num,
                    user_type = user.user_type,
                    city_id = user.city_id,
                    age = user.age,
                    information = user.information,
                });
            }
            catch (Exception err)
            {
                return Enumerable.Empty<User>();
            }
        }

        // Return an image as base64String to display it in the app or web
        [HttpGet("ProfileImage_")]
        public string GetUserImage(string id)
        {
            try
            {
                using (var context = new Database())
                {
                    var user = context.Users.Single(user => user.id == id);
                    if (user.image != null)
                    {
                        return Convert.ToBase64String(user.image);
                    }
                    return null;
                }
            }
            catch (Exception err)
            {
                return null;
            }
        }

        // Return the history records for one user
        [HttpGet("UserHistory_")]
        public IEnumerable<History> GetUserHistory(string id)
        {
            using (var context = new Database())
            {
                try
                {
                    if (context.Users.Any(user => user.id == id))
                    {
                        History history = _noSqlDatabase.GetHistory(context.Users.Single(user => user.id == id).record_doc);
                        foreach (var item in history.records)
                        {
                            var place = context.Places.Single(place => place.id == item.place_id);
                            item.place_id = place.name;
                        }
                        return Enumerable.Range(0, 1).Select(index => new History
                        {
                            records = history.records,
                        });
                    }
                    return Enumerable.Empty<History>();

                }
                catch (Exception err)
                {
                    return Enumerable.Empty<History>();
                }
            }
        }

        [HttpGet("ListOfSchdls_")]
        public IEnumerable<Schedules> GetUserListofSchdls(string user_id)
        {
            try
            {
                List<string> docs = new List<string>();
                var schedules = new List<Schedules>();
                using (var context = new Database())
                {
                    int count = context.Schedules.Count(usr => usr.user_id == user_id);
                    if (count != 0)
                    {
                        foreach (var schdl in context.Schedules)
                        {
                            if(schdl.user_id == user_id)
                                docs.Add(schdl.Doc_id);
                        }
                        schedules = _noSqlDatabase.GetUserSchedules(docs);
                        foreach(var schdl in schedules)
                        {
                            foreach(var day in schdl.days)
                            {
                                foreach (var hour in day.hours)
                                    hour.place_id = context.Places.Single(plc => plc.id == hour.place_id).name;                        
                            }
                            schdl.city_id = context.Cities.Single(cty => cty.id == schdl.city_id).name;
                        }
                    }
                }
                return schedules;                   
            }
            catch (Exception err)
            {
                return Enumerable.Empty<Schedules>();
            }
        }
        
        /* End of users functions */



        /* Function that return data related to places */

        // Return all places for admin, or if any user search for places
        [HttpGet("Places_")]
        public IEnumerable<Place> GetPlaces(string city_id, string place_type)
        {
            try
            {

                // 2- Select Many places within specific city
                int i = 0;
                List<Place> places = new List<Place>();
                using (var context = new Classes.Database())
                {
                    foreach (var item in context.Places)
                    {
                        if (item.city_id == city_id && item.place_type == place_type)
                        {
                            i++;
                            places.Add(item);
                        }
                    }
                    foreach (var item in places)
                        item.city_id = context.Cities.Single(c => c.id == city_id).name;
                }
                return Enumerable.Range(0, i).Select(Index => new Place
                {
                    id = places[Index].id,
                    location = places[Index].location,
                    description = places[Index].description,
                    name = places[Index].name,
                    place_type = places[Index].place_type,
                    city_id = places[Index].city_id,
                    
                }).ToArray();

            }
            catch (Exception err)
            {
                // error, noway to get the error except if the client doesn't define values for the parameters
                return Enumerable.Range(0, 1).Select(Index => new Place
                {
                    name = err.Message,
                    description = "you must set a value for place or city..."
                }).ToArray();
            }


        }

        // Return one place information in details
        [HttpGet("Place_")]
        public IEnumerable<Place> GetPlace(string palce_id)
        {
            try
            {
                // Select one place
                Place place = null;
                using (var context = new Database())
                {
                    place = context.Places.Single(item => item.id == palce_id);
                }

                // return will cuase ignoring to remaining code...
                return Enumerable.Range(0, 1).Select(Index => new Place
                {
                    name = place.name,
                    location = place.location,
                    totl_likes = place.totl_likes,
                    totl_visits = place.totl_visits,
                    crowd_status = place.crowd_status,
                    description = place.description,

                }).ToArray();

            }
            catch (Exception err)
            {
                // error, noway to get the error except if the client doesn't define values for the parameters
                return Enumerable.Range(0, 1).Select(Index => new Place
                {
                    name = err.Message,
                    description = "you must set a value for place or city... "
                }).ToArray();
            }
        }

        // Return random image as base64String
        [HttpGet("PlaceImage_")]
        public string GetPlaceImage(string place_id)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Places.Any(place => place.id == place_id))
                    {
                        string doc = context.Places.Single(place => place.id == place_id).related_doc;
                        return _noSqlDatabase.GetImage_Random(doc);
                    }
                }
                    return "No place with this ID";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        // Return users comments for one place
        [HttpGet("PlaceComments_")]
        public IEnumerable<Reviewer> GetComments(string place_id)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Places.Any(place => place.id == place_id))
                    {
                        var reviewers = _noSqlDatabase.GetComments(context.Places.Single(place => place.id == place_id).related_doc);
                        int i = 0;
                        foreach(var item in reviewers)
                        {
                            item.user_id = context.Users.Single(user => user.id == item.user_id).name;
                            i++;
                        }
                        return Enumerable.Range(0, i).Select(Index => new Reviewer
                        {
                            user_id = reviewers[Index].user_id,
                            reviews = reviewers[Index].reviews
                        }).ToArray();
                    }

                }

                return Enumerable.Empty<Reviewer>();
            }
            catch (Exception err)
            {
                return Enumerable.Empty<Reviewer>();
            }
        }

        [HttpGet("Cities_")]
        public IEnumerable<City> GetCities()
        {
            try
            {
                List<City> cities = new List<City>();
                int i = 0;

                using (var context = new Database())
                {
                    foreach (var city in context.Cities)
                    {
                        cities.Add(city);
                        i++;
                    }
                }
                return Enumerable.Range(0, i).Select(index => new City
                {
                    id = cities[index].id,
                    name = cities[index].name,
                });
            }
            catch (Exception err)
            {
                return Enumerable.Empty<City>();
            }
        }
        /* End of places functions */


        /* Functions to return the dataset for ML developer */
        /* Using the framework of RavenDB to call Map-Reduce */

        // Return Linear Regression dataset
        [HttpGet("LR_DataSet_")]
        public IEnumerable<DataSet.LinearRegression> GetDataSet_LR()
        {
            var dataset = _noSqlDatabase.Get_LR_DataSet();
            return dataset;
        }

        // Return Matrix Factorization dataset
        [HttpGet("MF_DataSet_")]
        public IEnumerable<DataSet.MatrixFactorization> GetDataSet_MF()
        {
            var dataset = _noSqlDatabase.Get_MF_DataSet();
            return dataset;
        }

        [HttpGet("Summation_")]
        public IEnumerable<int> GetTotal_ofEntities()
        {
            try
            {
                using (var context = new Database())
                {
                    return new int[]
                    {
                        context.Users.Count(user => user.user_type == false),
                        context.Users.Count(user => user.user_type == true),
                        context.Places.Count(),
                        context.Cities.Count(),
                        (context.Requests.Count(req => req.req_status == 0)// if 0, then its new request
                        +
                        context.Modifications.Count(modi => modi.operation.Contains("New")))
                    };
                }
            }
            catch (Exception err)
            {
                return new List<int>();
            }
        }



        /* Functions return data of relational tables in the database */
      
        // Get ads of one City (images) (its done between admins and cities)
        [HttpGet("Ads_")]
        public IEnumerable<string> GetAds(string city_id)
        {
            try
            {
                using (var context = new Database())
                {
                    if(context.Ads.Any(ad => ad.city_id == city_id))
                    {
                        List<string> ads = new List<string>();
                        foreach(var ad in context.Ads)
                        {
                            if (ad.city_id == city_id)
                                ads.Add(Convert.ToBase64String(ad.ad_image));
                        }
                        return ads;
                    }
                }
                return new string[] { "no images" };

            }
            catch (Exception err)
            {
                return new string[] { err.Message.ToString() };
            }
        }

        // Get all Requests between users and admins
        [HttpGet("Requests_")]
        public IEnumerable<Request> GetRequests()
        {
            try
            {
                List<Request> requests = new List<Request>();
                using (var context = new Database())
                {
                    foreach (var req in context.Requests)
                        requests.Add(req);

                    foreach(var user in context.Users)
                    {
                        foreach (var req in requests)
                        {
                            if (user.id == req.user_id)
                                req.user_id += ","+user.email;
                        }
                    }

                    foreach(var adm in context.Admin)
                    {
                        foreach(var req in requests)
                        {
                            if(adm.id == req.admin_id)
                                req.admin_id = adm.email;
                        }
                    }
                }
                return requests;
            }
            catch (Exception err)
            {
                return new List<Request>();
            }
        }

        // Get all Modification on the system between users and admins
        [HttpGet("Modifications_")]
        public IEnumerable<Modification> GetModifications()
        {
            try
            {
                List<Modification> modifications = new List<Modification>();
                using (var context = new Database())
                {
                    foreach (var modi in context.Modifications)
                        modifications.Add(modi);

                    foreach (var user in context.Users)
                    {
                        foreach (var modi in modifications)
                        {
                            if (user.id == modi.user_id)
                                modi.user_id += ","+user.email;
                        }
                    }

                    foreach (var adm in context.Admin)
                    {
                        foreach (var modi in modifications)
                        {
                            if (adm.id == modi.admin_id)
                                modi.admin_id = adm.email;
                        }
                    }
                }
                return modifications;
            }
            catch (Exception err)
            {
               return null;
            }
        }



    }
}
