using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace Models
{
    public class SecureArea
    {
        public int id;
        public IList<Position> area;
        public string creater;
        public DateTime createtime;
        public string information;
        public bool status;
    }
}
