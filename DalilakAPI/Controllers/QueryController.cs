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
                        string base64String = Convert.ToBase64String(user.image, 0, user.image.Length);
                        return base64String;
                    }
                    return null;
                }
            }
            catch (Exception err)
            {
                return null;
            }
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
        public IEnumerable<Admin> getAdmins(string admin_id)
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

        // http Request to select one elemnt if the client post the place id,
        // and so on select all of the places on specific city if the client set the city id
        [HttpGet("Places_")]
        public IEnumerable<Place> getPlaces(string palce_id, string city_id, string NAT, string HIS, string EVE)
        {
            try
            {
                if (palce_id != null)
                {
                    // 1- Select one place
                    Place place = null;
                    using (var context = new Classes.Database())
                    {
                        place = context.Places.Single(item => item.id == palce_id || item.city_id == city_id);
                    }

                    // return will cuase ignoring to remaining code...
                    return Enumerable.Range(0, 1).Select(Index => new Place
                    {
                        name = place.name,
                        location = place.location,
                        place_type = place.place_type,
                        totl_likes = place.totl_likes,
                        totl_visits = place.totl_visits,
                        crowd_status = place.crowd_status,
                        description = place.description,

                    }).ToArray();
                }

                // 2- Select Many places within specific city
                int i = 0;
                List<Place> places = new List<Place>();
                using (var context = new Classes.Database())
                {
                    foreach (var item in context.Places)
                    {
                        if (item.city_id == city_id)
                        {
                            i++;
                            places.Add(item);
                        }
                    }
                }
                return Enumerable.Range(0, i).Select(Index => new Place
                {
                    name = places[Index].name,
                    location = places[Index].location,
                    place_type = places[Index].place_type,
                    totl_likes = places[Index].totl_likes,
                    totl_visits = places[Index].totl_visits,
                    crowd_status = places[Index].crowd_status,
                    description = places[Index].description,

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
    }
}
