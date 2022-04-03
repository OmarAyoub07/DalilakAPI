using DalilakAPI.Classes;
using DalilakAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using System;
using System.Collections.Generic;

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
