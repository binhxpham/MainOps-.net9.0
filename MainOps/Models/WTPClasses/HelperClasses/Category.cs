using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MinLength(1)]
        [Display(Name ="Category")]
        public string category { get; set; }
    }
}
