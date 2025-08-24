using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class Item_Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string Item_category { get; set; }
    }
}
