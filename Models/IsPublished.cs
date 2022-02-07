using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class IsPublished
    {
        [Key]
        public int ID { get; set; }

        public string ConnectionId { get; set; }
    }
}
