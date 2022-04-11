using System.Collections.Generic;

namespace DalilakAPI.Models
{
    public class Statistics
    {
        public string place_id { get; set; }
        public List<VisitDay> days { get; set; }
        public string Id { get; set; }

    }
    public class VisitDay
    {
        public string date { get; set; }
        public List<VisitTime> hours { get; set; }
    }
    public class VisitTime
    {
        public string time { get; set; }
        public int visits_num { get; set; }

    }
}

