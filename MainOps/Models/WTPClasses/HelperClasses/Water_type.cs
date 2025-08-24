using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Water_type
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display(Name = "Water Type")]
        public string water_type { get; set; }

    }
}
