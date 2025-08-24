using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Collections.Generic;
using MainOps.Models.WTPClasses.HelperClasses;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class WTP_block
    {
        [Key]
        public int id { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name should not be more than 100 characters and also not empty")]
        [Display(Name = "Name")]
        public string name { get; set; }
        [DisplayName("Size")]
        [Required]
        public double size { get; set; }
        [ForeignKey("Unit")]
        public int? unit_sizeid { get; set; }
        [DisplayName("Unit")]
        public virtual WTPUnit unit_size { get; set; } 
        [DisplayName("Length")]
        public double? length { get; set; }
        [DisplayName("Width")]
        public double? width { get; set; }
        [DisplayName("Height")]
        public double? height { get; set; }
        [DisplayName("Weight")]
        public double? weight { get; set; }
        [DisplayName("Power Consumption")]
        public double? Pow_Con { get; set; }
        [DisplayName("Unit Power Consumption")]
        [ForeignKey("WTPUnit")]
        public int? WTPUnitId { get; set; }
        [DisplayName("Unit")]
        public virtual WTPUnit WTPUnit { get; set; }
        [DisplayName("Necessity")]
        public bool necessity { get; set; }
        [ForeignKey("Division")]
        public int DivisionId { get; set; }
        public Division Division { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        public string uniquename()
        {
            string[] s = { name, size.ToString() };
            return string.Join("_",s);
        }
    }
}
