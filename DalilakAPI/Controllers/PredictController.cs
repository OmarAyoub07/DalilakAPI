using DalilakAPI.Classes;
using DalilakAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DalilakAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly ILogger<PredictController> _logger;

        private MLContext mlContext = new MLContext();
        

        private ITransformer LR_model;
        private ITransformer MF_mldel;

        private string LR_ModelPath = "MachineLearning Models/LR_Model.zip";
        private string MF_ModelPath = "MachineLearning Models/MF_Model.zip";

        private PredictionEngine<DataSet.LinearRegression, DataSet.LR_Predictions> LR_predictionEngin;
        private PredictionEngine<DataSet.MatrixFactorization, DataSet.Recommender_Predictions> MF_predictionEngin;

        public PredictController(ILogger<PredictController> logger)
        {
            _logger = logger;
            loadModels();
        }


        [HttpGet("LR_SinglePrediction_")]
        public string LR_Predict1(string place_id, int month, int dayOfweek, Single time)
        {
            string date = DayText(dayOfweek);
            date += MonthText(month);

            // set a scenario to pedict it
            var visitSample = new DataSet.LinearRegression()
            {
                place_id = place_id,
                date = date,
                time = time
            };
            // Engine to prediction process
            return LR_predictionEngin.Predict(visitSample).visits_num.ToString();
        }
        [HttpPost("LR_MultiPredicts")]
        public IEnumerable<string[]> LR_MultiPredicts(string[] places_id, DateTime now)
        {
            try
            {
                List<string[]> list = new List<string[]>();
                string date = DayText((int)now.DayOfWeek+1);
                date += MonthText(now.Month);

                for (int i = 0; i < places_id.Length; i++)
                {
                    try
                    {
                        float time = now.Hour;
                        for (int j = 0; j < 7; j++)
                        {
                            
                            var visitSample = new DataSet.LinearRegression()
                            {
                                place_id = places_id[i],
                                date = date,
                                time = time
                            };

                            list.Add(new string[] { places_id[i], LR_predictionEngin.Predict(visitSample).visits_num.ToString() });
                            time+=3f;
                            time = time % 24;
                        }
                    }
                    catch
                    {
                        list.Add(new string[] {"fail"});
                        continue;
                    }
                }

                return list;
            }
            catch(Exception err)
            {
                return null;
            }
        }



        [HttpGet("MF_SinglePrediction_")]
        public string MF_Predict1(string place_id, string user_id)
        {
            var recomSample = new DataSet.MatrixFactorization { user_id = user_id, place_id = place_id };

            return MF_predictionEngin.Predict(recomSample).Score.ToString();
        }

        [HttpGet("MF_GetTotalRecommendation")]
        public IEnumerable<int> MF_TotalRecom()
        {
            int totl_recom = 0;
            int totl_notRecom = 0;
            int totl_cantPredict = 0;

            using (var context = new Database())
            {
                List<User> users = new List<User>();
                foreach(var user in context.Users)
                {
                    users.Add(user);
                }
                foreach (var place in context.Places)
                {
                    foreach (var user in users)
                    {
                        try
                        {
                            var recomSample = new DataSet.MatrixFactorization { user_id = user.id, place_id = place.id };
                            var rate = MF_predictionEngin.Predict(recomSample).Score;
                            if (rate > 0.55)
                                totl_recom++;
                            else if(float.IsNaN(rate))
                                totl_cantPredict++;
                            else
                                totl_notRecom++;
                        }
                        catch
                        {
                            totl_cantPredict++;
                            continue;
                        }
                    }
                }
            }

            return new int[] { totl_recom, totl_notRecom, totl_cantPredict };
        }

        [HttpGet("GenerateSchedule_")]
        public IEnumerable<Schedules> GenerateSchdl(string user_id, string cityID, string fromDate, string toDate, float crowdRate)
        {
            // Recommender Function to generate new plan for user in specific city
            try
            {
                DateTime fromDateTime = DateTime.Parse(fromDate); // Date posted by user 
                int toDateTime = DateTime.Parse(toDate).Month; // example from 1/1/2022 to 4/2/2022

                List<Place> Recommended_places = new List<Place>(); // Store Recommended places for user
                Schedules schedule = new Schedules(); // Store the final result (Trip Plan)

                using (var context = new Database())
                {
                    if (context.Users.Any(user => user.id == user_id))
                    {
                        schedule.user_id = user_id; // assign user id
                        schedule.city_id = cityID; // Assign City Id
                        schedule.days = new List<TripDay>(); // Declare list for days

                        /* Recommender loop */
                        foreach(var place in context.Places.Where(plc => plc.city_id == cityID))
                        {
                            // Predict Recommended places for the user
                            var recomSample = new DataSet.MatrixFactorization { user_id = user_id, place_id = place.id };
                            var rate = MF_predictionEngin.Predict(recomSample).Score;

                            if (rate > 0.65) // Is Recommended 
                                Recommended_places.Add(place);
                        }

                        List<string> stored_places = new List<string>(); // List to store places added to the plan

                        /* Predict visits rate based on posted percentage */
                        for (int month = fromDateTime.Month; month <= toDateTime; month+=2) // Months loop 
                        {
                            for (int dayOfweek = 1; dayOfweek <= 7; dayOfweek++) // Days loop
                            {
                                string date = DayText(dayOfweek);
                                date += MonthText(month); // convert date to form that machine learning can understand

                                List<int> predicted_Times_Perday = new List<int>(); // List to store times added to the plan (for one day)
                                                                                    // to avoid the repetitions (to avoid adding multi places on same time)

                                /* loop recommended places */
                                foreach (var place in Recommended_places)
                                {
                                    for (int time = 0; time < 24; time+=3) // Hours loop
                                    {
                                        /* Predict Visits Rate */
                                        var visitSample = new DataSet.LinearRegression()
                                        {
                                            place_id = place.id,
                                            date = date,
                                            time = time
                                        };
                                        var predict = LR_predictionEngin.Predict(visitSample).visits_num; // predicted value

                                        /* if statement - to make judgment (is it recommended or not ) */
                                        // 1 - if the visits Rate less than predicted
                                        //      * if the user want the most visits time or best time, he should post greater the (0.5)
                                        // 2 - and if the system doesn't recommend this place before
                                        //      * if the system recommend (Al-Hada), it shouldn't recommend (Al-Hada) again
                                        // 3 - and if the system doesn't recommend this time before (for one day)
                                        //      * if the system recommend for any place at (9:00 - sunday), it shouldn't recommend any place at (9:00 - sunday)
                                        if (crowdRate <= predict & (!predicted_Times_Perday.Any(t => t == time)) & (!stored_places.Any(p => p == place.id)))
                                        {
                                            string date1 = fromDateTime.ToString("MM/dd/yyyy"); // get date of recommendation as string

                                            /* add recommended place to the user plan */
                                            if (!schedule.days.Any(d => d.date == date1)) // if the schedule dont contains of date equals to (date1)                                           
                                                schedule.days.Add(new TripDay { date = date1, hours = new List<TripTime>() }); // adding date                                            
                                            
                                                var day = schedule.days.Single(d => d.date == date1); // Get
                                                day.hours.Add(new TripTime { time = time.ToString(), place_id = place.id }); // add time
                                            
                                            stored_places.Add(place.id); // store place id as recommended place, and should not be recommended again
                                            predicted_Times_Perday.Add(time); // store this time as recommended time for this day
                                            break; // break loop, no need to continue while the place is recommended
                                        }// if end
                                    }// time loop end
                                }// recommended places loop end
                                fromDateTime = fromDateTime.AddDays(3);
                            } // days loop end
                            fromDateTime = fromDateTime.AddMonths(2); // Inceremnt months
                        } // months loop end
                    }// if end

                    // Make a copy of object without reference
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(schedule);
                    Schedules user_schdl = Newtonsoft.Json.JsonConvert.DeserializeObject<Schedules>(json);

                    /* Convert the schedule to form that user can read */
                    user_schdl.city_id = context.Cities.Single(c => c.id == cityID).name; // Get City Name
                    foreach (var day in user_schdl.days)
                        foreach (var hour in day.hours)
                        {
                            hour.place_id = context.Places.Single(p => p.id == hour.place_id).name;
                        }

                    return new List<Schedules>() { user_schdl , schedule };
                }
            }
            catch (Exception err)
            {
                return new List<Schedules>();
            }
}

        private void loadModels()
        {
            DataViewSchema modelSchema;
            // load Models
            LR_model = mlContext.Model.Load(LR_ModelPath, out modelSchema);
            MF_mldel = mlContext.Model.Load(MF_ModelPath, out modelSchema);

            // Engine to prediction process for both models
            LR_predictionEngin = mlContext.Model.CreatePredictionEngine<DataSet.LinearRegression, DataSet.LR_Predictions>(LR_model);
            MF_predictionEngin = mlContext.Model.CreatePredictionEngine<DataSet.MatrixFactorization, DataSet.Recommender_Predictions>(MF_mldel);
        }

        private string MonthText(int month)
        {
            string date = "";
            switch (month)
            {
                case <= 2:
                    date+=" January";
                    break;
                case <=4:
                    date+=" March";
                    break;
                case <=6:
                    date+=" May";
                    break;
                case <=8:
                    date+=" July";
                    break;
                case <=10:
                    date+=" September";
                    break;
                case <=12:
                    date+=" November";
                    break;
            }
            return date;
        }

        private string DayText(int day)
        {
            string date = "";
            switch (day)
            {
                case 1:
                    date = "Sunday -";
                    break;
                case 2:
                    date = "Monday -";
                    break;
                case 3:
                    date = "Tuesday -";
                    break;
                case 4:
                    date = "Wednesday -";
                    break;
                case 5:
                    date = "Thursday -";
                    break;
                case 6:
                    date = "Friday -";
                    break;
                case 7:
                    date = "Saturday -";
                    break;
            }
            return date;
        }
    }
}
