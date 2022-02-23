using System.Collections.Generic;

namespace DalilakAPI.Models
{
    public class History
    {
        public string user_id { get; set; }
        public List<Record> records { get; set; }
        public string Id { get; set; }
    }

    public class Record
    {
        public string place_id { get; set; }
        public float rate { get; set; }
        public bool favorite { get; set; }
    }
}

