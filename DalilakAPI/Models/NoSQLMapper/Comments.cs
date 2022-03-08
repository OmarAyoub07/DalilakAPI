using System.Collections.Generic;

namespace DalilakAPI.Models
{
    public class Comments
    {
        public string place_id { get; set; }
        public List<string> images { get; set; }
        public List<Reviewer> reviewers { get; set; }
        public string Id { get; set; }

    }
    public class Reviewer
    {
        public string user_id { get; set; }
        public bool like { get; set; }
        public List<Review> reviews { get; set; }

    }
    public class Review
    {
        public string comment { get; set; }
        public string date { get; set; }
        public string time { get; set; }
    }
}

