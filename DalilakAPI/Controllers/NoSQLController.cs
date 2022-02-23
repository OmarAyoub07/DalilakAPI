using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using DalilakAPI.Models;

namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NoSQLController : ControllerBase
    {
        private Classes.NoSqlDatabase nosql = new Classes.NoSqlDatabase();

        private readonly ILogger<NoSQLController> _logger;

        public NoSQLController(ILogger<NoSQLController> logger)
        {
            _logger = logger;
        }


        [HttpGet("Statistics")]
        public Statistics getStatistics()
        {
            var statistics = nosql.selectStatistics("643c9b28-5458-41e0-b80d-0eb3ef583058");

            return new Statistics { place_id = statistics.place_id, Id = statistics.Id, days = statistics.days };
        }

        [HttpGet("Comments")]
        public IEnumerable<Comments> getComments()
        {
            try
            {
                var comments = nosql.selectComments();

                return Enumerable.Range(0, comments.Count).Select(index => new Comments
                {
                    Id = comments[index].Id,
                    place_id = comments[index].place_id,
                    images = comments[index].images,
                    reviewers = comments[index].reviewers

                }).ToArray();
            }
            catch(Exception err)
            {
                return Enumerable.Range(0, 1).Select(Index => new Comments
                {
                    Id= err.Message,
                }).ToArray();
            }
        }

        [HttpGet("History")]
        public IEnumerable<History> getHistory()
        {
            var histories = nosql.selectHistories();

            return Enumerable.Range(0, histories.Count).Select(index => new History
            {
                Id = histories[index].Id,
                user_id = histories[index].user_id,
                records = histories[index].records

            }).ToArray();
        }

        [HttpGet("Schedules")]
        public IEnumerable<Schedules> getSchedules()
        {
            var schedules = nosql.selectSchedules();

            foreach (var item in schedules)
            {
                item.user_id = "Not Allowed";
                item.city_id = "Not Allowed";
                item.Id = "Not Allowed";

                foreach (var subItem in item.days)
                {
                    foreach (var sub2Item in subItem.hours)
                        sub2Item.place_id = "Not Allowed";
                }
            }

            return Enumerable.Range(0, schedules.Count).Select(index => new Schedules
            {
                Id = schedules[index].Id,
                user_id = schedules[index].user_id,
                city_id = schedules.First().city_id,
                days = schedules[index].days,
            }).ToArray();
        }

    }
}
