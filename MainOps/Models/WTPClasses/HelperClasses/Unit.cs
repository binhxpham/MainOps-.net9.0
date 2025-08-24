using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class WTPUnit
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display (Name = "Unit")]
        public string? the_unit { get; set; }
    }
}
