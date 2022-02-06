using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace Models
{
    public class SecureArea
    {

        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Latitude { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Longitude { get; set; }

        //[NotMapped]
        //public int id { get; set; }
        [NotMapped]
        public IList<MyPosition> area { get; set; }
        [NotMapped]
        public string createrid { get; set; }//替換爲createraccount
        [NotMapped]
        public string wardid { get; set; }//替換爲wardaccount

        //[Key]
        //public int ID { get; set; }
        public string creatername { get; set; }
        public DateTime createtime { get; set; }
        public string information { get; set; }
        public bool status { get; set; }
        public string wardname { get; set; }


        public string wardaccount { get; set; }
        public string createraccount { get; set; }
        public string areaid { get; set; }
        

    }
}
