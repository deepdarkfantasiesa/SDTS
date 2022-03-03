using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Invitation
    {
        [Key]
        public int ID { get; set; }
        public string InviterAccount { get; set; }
        public string InviteCode { get; set; }
    }
}
