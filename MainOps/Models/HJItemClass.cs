using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class HJItemClass
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Item Class")]
        public string ClassName { get; set; }
        public string ClassNumber { get; set; }
        [ForeignKey("HJItemMasterClass")]
        public int? HJItemMasterClassId { get; set; }
        public virtual HJItemMasterClass HJItemMasterClass { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? Service_Maintenance_Freq { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? Safety_Maintenance_Freq { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? Electrical_Maintenance_Freq { get; set; }
        [Display(Name = "Internal Rent")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double Internal_Rent { get; set; }
        public string NameAndNumber { get
            {
                return String.Concat(this.ClassNumber, " : ", this.ClassName);
            } }
    }
}
