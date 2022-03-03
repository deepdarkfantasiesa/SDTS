using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EmergencyHelper
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Information { get; set; }

        public string Account { get; set; }

        public string Gender { get; set; }

        public DateTime Birthday { get; set; }

        public string PhoneNumber { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime dateTime { get; set; }
        public string ConnectionId { get; set; }
        public string Problem { get; set; }
        public string GroupName { get; set; }

        public double Altitude { get; set; }

    }
}
