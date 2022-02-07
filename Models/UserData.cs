using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class UserData
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
    }
}
