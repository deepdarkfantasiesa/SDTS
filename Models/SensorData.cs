using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Models
{
    public class SensorData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double AccDataX { get; set; }
        public double AccDataY { get; set; }
        public double AccDataZ { get; set; }

        public List<Tuple<double,double,double>> dataAcc { get; set; }
        public List<double> dataBar { get; set; }
        public List<Tuple<double, double, double>> dataGyr { get; set; }
        public List<Tuple<double, double, double>> dataMag { get; set; }
        public List<Tuple<double, double, double>> dataOri { get; set; }

        public User user { get; set; }

        public DateTime dateTime { get; set; }
    }
}
