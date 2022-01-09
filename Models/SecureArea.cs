﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace Models
{
    public class SecureArea
    {
        public int id { get; set; }
        public IList<Position> area { get; set; }
        public string creater { get; set; }
        public DateTime createtime { get; set; }
        public string information { get; set; }
        public bool status { get; set; }
        public string wardid { get; set; }
    }
}
