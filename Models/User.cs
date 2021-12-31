using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string Name { get; set; }

        public string Information { get; set; }

        public string password { get; set; }

        public string type { get; set; }

        public string gender { get; set; }

        public DateTime birthday { get; set; }

        public static implicit operator Task<object>(User v)
        {
            throw new NotImplementedException();
        }
    }
}
