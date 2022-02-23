
using Microsoft.EntityFrameworkCore;
using DalilakAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace DalilakAPI.Classes
{
    public class Database : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Modification> Modifications { get; set; }
        public DbSet<Ad> Ads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schedule>().HasNoKey();
            modelBuilder.Entity<Request>().HasNoKey();
            modelBuilder.Entity<Modification>().HasNoKey();
            modelBuilder.Entity<Ad>().HasNoKey();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server = n1nlmysql23plsk.secureserver.net; Port = 3306; Database = Dalilak_DB; Uid = CP_TU; Pwd = nc3V2x@8;");
        }

    }
}