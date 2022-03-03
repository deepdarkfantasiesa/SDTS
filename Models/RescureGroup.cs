﻿using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RescureGroup
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string ConnectionId { get; set; }
        public string GroupName { get; set; }
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
        //public DateTime dateTime { get; set; }
        //public double BarometerData { get; set; }
    }
}
