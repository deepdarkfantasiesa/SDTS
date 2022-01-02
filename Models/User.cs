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

        public string Account { get; set; }

        public string PassWord { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public DateTime Birthday { get; set; }

        public string PhoneNumber { get; set; }

        public List<string> EmergencyContacts { get; set; }

        public static implicit operator Task<object>(User v)
        {
            throw new NotImplementedException();
        }
    }
}
