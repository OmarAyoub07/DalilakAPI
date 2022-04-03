using Microsoft.ML.Data;
using System;

public class DataSet
{
    public class LinearRegression
    {
        public string place_id { get; set; }
        public string date { get; set; }
        public Single time { get; set; }
        public int visits_num { get; set; }     
    }

    public class LR_Predictions
    {
        [ColumnName("Score")]
        public float visits_num;
    }


    public class MatrixFactorization
    {
        public string user_id { get; set; }
        public string place_id { get; set; }
        public Single rate { get; set; }
    }
    public class Recommender_Predictions
    {
        public float Score { get; set; }
    }
}

