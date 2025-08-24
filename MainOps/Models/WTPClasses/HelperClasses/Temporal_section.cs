using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Temporal_section
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display(Name = "Temporal Section")]
        public string section { get; set; }
    }
}
