using MainOps.Models.WTPClasses.HelperClasses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class FilterMaterial
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(120, MinimumLength = 1, ErrorMessage = "Name should not be more than 120 characters and also not empty")]
        [Display(Name = "Filtermaterial")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Contaminations")]
        public string contaminations { get; set; }
        [Required]
        [Display(Name = "Device")]
        public string device { get; set; }
        [ForeignKey("Water_type")]
        public int? water_typeid { get; set; }
        [Display(Name = "Water Type")]
        public virtual Water_type water_type { get; set; }
        
    }
    

}
