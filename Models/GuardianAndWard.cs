using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class GuardianAndWard
    {
        [Key]
        public int ID { get; set; }

        public string GuardianAccount { get; set; }
        public string WardAccount { get; set; }
    }
}
