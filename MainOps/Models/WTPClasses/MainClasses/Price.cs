using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MainOps.Models.WTPClasses.HelperClasses;
using MainOps.Models.WTPClasses.MixedClasses;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class Price
    {
        [Key]
        public int id { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name should not be more than 100 characters and also not empty")]
        [Display(Name = "English Name")]
        public string name { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int EkdTid { get; set; }
        public virtual Category EkdT { get; set; }
        [Display(Name = "Size")]
        public double? size { get; set; }
        [ForeignKey("Unit")]
        [Display(Name = "Unit size")]
        public int unitid { get; set; }
        public virtual WTPUnit unit { get; set; }
        [DataType(DataType.Currency)]
        [Display(Name = "Price per piece")]
        public double? price { get; set; }
        [ForeignKey("Unit")]
        [Display(Name = "Price Unit")]
        public int unit_pid { get; set; }
        [Display(Name = "Price Unit")]
        public virtual WTPUnit unit_p { get; set; }
        [ForeignKey("Unit")]
        [Display(Name = "Price Unit")]
        public int unit_rid { get; set; }        
        [Display(Name = "Rental Price")]
        [DataType(DataType.Currency)]
        public double? rent { get; set; }
        [Display(Name = "Rental Unit")]
        public virtual WTPUnit unit_r { get; set; }
        [ForeignKey("Division")]
        public int divisionid { get; set; }
        [Display(Name="Division")]
        public virtual Division division { get; set; }
        public Price()
        {

        }
        public Price(EffortPrice effortPrice)
        {
            name = effortPrice.effort.Name;
            EkdTid = effortPrice.pxx.EkdTid;
            size = effortPrice.pxx.size;
            price = effortPrice.pxx.price;
            unit_pid = effortPrice.pxx.unit_pid;
            rent = effortPrice.pxx.rent;
            unit_rid = effortPrice.pxx.unit_rid;
            divisionid = effortPrice.pxx.divisionid;
            unitid = effortPrice.pxx.unitid;
        }
    }
}
