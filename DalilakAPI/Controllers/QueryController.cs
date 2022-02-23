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

        // Get All Users from database
        [HttpGet("users_")]
        public IEnumerable<User> getUsers(string user_id)
        {
            try
            {
                if (user_id != null)
                {
                    User user = null;
                    using (var context = new Database())
                    {
                        user = context.Users.Single(item => item.id == user_id);
                    }

                    return Enumerable.Range(0, 1).Select(index => new User
                    {
                        name = user.name,
                        email = user.email,
                        age = user.age,
                        city_id = user.city_id,
                        phone_num = user.phone_num,
                        image = user.image,
                        information = user.information,
                        user_type = user.user_type,
                    });
                }

                List<User> users = new List<User>();
                int i = 0;
                using (var context = new Database())
                {
                    foreach (var item in context.Users)
                    {
                        i++;
                        users.Add(item);
                    }
                }

                return Enumerable.Range(0, i).Select(index => new Models.User
                {
                    name = users[index].name,
                    email = users[index].email,
                    age = users[index].age,
                    city_id = users[index].city_id,
                    phone_num = users[index].phone_num,
                    image = users[index].image,
                    information = users[index].information,
                    user_type = users[index].user_type,
                }).ToArray();
            }
            catch (Exception err)
            {
                return Enumerable.Empty<User>();
            }
        }

        // Get All rows from Admin Table at database
        [HttpGet("admin_")]
        public IEnumerable<Admin> getAdmins(string admin_id)
        {
            try
            {
                if(admin_id != null)
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
        public IEnumerable<Place> getPlaces(string palce_id, string city_id)
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
