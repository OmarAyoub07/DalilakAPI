using Microsoft.EntityFrameworkCore;
using DalilakAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace DalilakAPI.Classes
{
    public class Database : DbContext
    {
        /* Add the classes to database context */

        public DbSet<Admin> Admin { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Modification> Modifications { get; set; }
        public DbSet<Ad> Ads { get; set; }

        /* The conditions below must be pass for all above variables */
        // If the variable name equal to any table name on the database (And)
        // If the attributes on the referenced class match all attributes on table (datatypes and names of columns)
        // Then, the connection will initialized, otherwise exception error will occur

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // One Connection string executed on configuration
            optionsBuilder.UseMySQL("Server = n1nlmysql23plsk.secureserver.net; Port = 3306; Database = Dalilak_DB; Uid = CP_TU; Pwd = nc3V2x@8;");
        }

    }
}