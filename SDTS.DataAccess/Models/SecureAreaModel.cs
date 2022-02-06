using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Models
{
    public class SecureAreaModel: SecureArea
    {
        //[Key]
        //public int ID { get; set; }
        //[Column(TypeName = "varchar(MAX)")]
        //public string Latitude { get; set; }
        //[Column(TypeName = "varchar(MAX)")]
        //public string Longitude { get; set; }
    }
}
