using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DalilakAPI.Models
{
    public class Admin
    {
        [Key]
        public string id { get; set; }
        //[Unique]
        public string email { get; set; }
        //[Unique]
        public string password { get; set; }
    }

    public class User
    {
        [Key]
        public string id { get; set; }
        public string phone_num { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public bool user_type { get; set; }
        public int age { get; set; }
        public byte[] image { get; set; }
        public string information { get; set; }
        public string record_doc { get; set; }

        [ForeignKey("FK_City_ToUsers")]
        public string city_id { get; set; }


    }

    public class Place
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string place_type { get; set; }
        public string crowd_status { get; set; }
        public string related_doc { get; set; }
        public string statstc_doc { get; set; }
        public int totl_likes { get; set; }
        public int totl_visits { get; set; }

        [ForeignKey("FK_City_ToPlace")]
        public string city_id { get; set; }

    }

    public class City
    {
        [Key]
        public string id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    public class Schedule
    {
        public string Doc_id { get; set; }

        [ForeignKey("FK_User_Trips")]
        public string user_id { get; set; }
    }

    public class Request
    {
        [ForeignKey("FK_admin_Response")]
        public string admin_id { get; set; }

        [ForeignKey("FK_user_Ask")]
        public string user_id { get; set; }
        public byte[] file { get; set; }
        public bool req_status { get; set; }

    }

    public class Modification
    {
        [ForeignKey("FK_admin_Confirm")]
        public string admin_id { get; set; }

        [ForeignKey("FK_user_Modify")]
        public string user_id { get; set; }
        public string operation { get; set; }
    }

    public class Ad
    {
        [ForeignKey("FK_Admin_Control")]
        public string admin_id { get; set; }

        [ForeignKey("FK_City_ToControl")]
        public string city_id { get; set; }

        public byte[] ad_image { get; set; }
    }
}
