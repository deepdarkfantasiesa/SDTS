﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class SensorsData
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Account { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime dateTime { get; set; }
        public string ConnectionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BarometerData { get; set; }

        [NotMapped]
        public List<Tuple<double, double, double>> dataAcc { get; set; }
        [NotMapped]
        public List<double> dataBar { get; set; }
        [NotMapped]
        public List<Tuple<double, double, double>> dataGyr { get; set; }
        [NotMapped]
        public List<Tuple<double, double, double>> dataMag { get; set; }
        [NotMapped]
        public List<Tuple<double, double, double>> dataOri { get; set; }
    }
}
