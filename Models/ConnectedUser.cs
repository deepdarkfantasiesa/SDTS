using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class ConnectedUser
    {
        [Key]
        public int ID { get; set; }
        public string Account { get; set; }
        public string ConnectId { get; set; }
    }
}
