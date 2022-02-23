using System.Collections.Generic;

namespace DalilakAPI.Models
{
    public class Schedules
    {
        // json objects properties
        public string user_id { get; set; }
        public string city_id { get; set; }
        public List<TripDay> days { get; set; }

        // json file uniqe id - to reach, update, delete the file
        public string Id { get; set; }
    }


    public class TripDay
    {
        public string date { get; set; }
        public List<TripTime> hours { get; set; }
    }

    public class TripTime
    {
        public string time { get; set; }

        // for schedules
        public string place_id { get; set; }

    }
}

