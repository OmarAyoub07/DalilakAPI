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

        /* Rest Functions to return data related to users*/
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
                        return user.image;
                    }
                    return null;
                }
            }
            catch (Exception err)
            {
                return null;
            }
        }

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
        /* End of users functions */




        /* Function that deal with places */

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
                }
                return Enumerable.Range(0, i).Select(Index => new Place
                {
                    id = places[Index].id,
                    name = places[Index].name

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
                    description = "you must set a value for place or city..."
                }).ToArray();
            }
        }

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

        [HttpGet("LR_DataSet_")]
        public IEnumerable<DataSet.LinearRegression> GetDataSet_LR()
        {
            var dataset = _noSqlDatabase.Get_LR_DataSet();
            return dataset;
        }

        [HttpGet("MF_DataSet_")]
        public IEnumerable<DataSet.MatrixFactorization> GetDataSet_MF()
        {
            var dataset = _noSqlDatabase.Get_MF_DataSet();
            return dataset;
        }



        /* Rest Functions to retun data related to cities */
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

        // Get All rows from Admin Table at database
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

    }
}
