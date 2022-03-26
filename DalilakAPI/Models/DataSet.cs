public class DataSet
{
    public class LinearRegression
    {
        public string place_id { get; set; }
        public string date { get; set; }
        public int time { get; set; }
        public int visits_num { get; set; }     
    }

    public class MatrixFactorization
    {
        public string user_id { get; set; }
        public string place_id { get; set; }
        public int rate { get; set; }
    }
}

