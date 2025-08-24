using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Atom
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string name { get; set; }
        [Required]
        [Range(0,200)]
        [Display(Name = "Masse")]
        public double mass { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(2)]
        [Display(Name = "Symbol")]
        public string symbol { get; set; }
    }
}
