using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Luxurity
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display(Name = "Luxurity")]
        public string wtp_luxurity { get; set; }
    }
}
