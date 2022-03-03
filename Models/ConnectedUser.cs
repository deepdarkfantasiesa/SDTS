using System.ComponentModel.DataAnnotations;

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
