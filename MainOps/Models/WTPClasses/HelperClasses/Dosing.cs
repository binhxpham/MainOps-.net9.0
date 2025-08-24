using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Dosing
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Dosing")]
        public string dosing { get; set; }
    }
}
