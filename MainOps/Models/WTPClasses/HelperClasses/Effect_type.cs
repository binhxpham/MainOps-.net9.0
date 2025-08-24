using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Effect_type
    {
        [Key]
        public int id { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }
        [Required]
        [Display(Name = "Path")]
        public string path_to_graph { get; set; }
    }
}
