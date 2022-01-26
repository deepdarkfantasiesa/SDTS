using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Helpers:User
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime dateTime { get; set; }

        public string ConnectionId { get; set; }

        public bool Published { get; set; }

        public string Problem { get; set; }

        public List<string> isPublished { get; set; }//存储已经发送过求助信息的志愿者和监护人
    }
}
