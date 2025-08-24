using System.ComponentModel.DataAnnotations;


namespace MainOps.Models.WTPClasses.HelperClasses
{
    public class FilterPrice
    {
        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Price")]
        public double price { get; set; }
    }
}
