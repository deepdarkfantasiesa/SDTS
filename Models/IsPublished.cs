﻿using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class IsPublished
    {
        [Key]
        public int ID { get; set; }

        public string Account { get; set; }
        public string ConnectionId { get; set; }
        public string HelperAccount { get; set; }
    }
}
