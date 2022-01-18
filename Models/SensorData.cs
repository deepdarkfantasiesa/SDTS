using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class SensorData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public User user { get; set; }
    }
}
