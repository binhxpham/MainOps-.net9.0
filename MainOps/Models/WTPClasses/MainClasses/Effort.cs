using MainOps.Models.WTPClasses.HelperClasses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class Effort
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [Display(Name = "Name")]
        public String Name { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(80)]
        [Display(Name = "WTP-Block")]
        public String WTP_block_name { get; set; }
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        [Display(Name = "Category")]
        public virtual Category Category { get; set; }
        [Required]
        [Range(0,10000)]
        [Display(Name = "Effort")]
        public double effort { get; set; }
        [ForeignKey("WTPUnit")]
        public int? WTPUnitId { get; set; }
        [Display(Name = "Unit")]
        public virtual WTPUnit WTPUnit { get; set; }
        [ForeignKey("Temporal_section")]
        public int? Temp_sectionid { get; set; }
        [Display(Name = "Temporal Section")]
        public virtual Temporal_section Temp_section { get; set; }
        [ForeignKey("Country")]
        public int? DivisionId { get; set; }
        [Display(Name = "Country")]
        public virtual Division Division { get; set; }
        [ForeignKey("Luxurity")]
        public int? Wtp_luxurityId { get; set; }
        [Display(Name = "Luxurity")]
        public virtual Luxurity Wtp_luxurity { get; set; }
        public string[] nameparts(string s)
        {
            char[] delimiterchar = { '_' };
            var s2 = s.Split(delimiterchar);
            return s2;
        }
        public Effort()
        {

        }
        public Effort(Effort e, string newName)
        {
            Name = e.Name;
            WTP_block_name = newName;
            CategoryId = e.CategoryId;
            effort = e.effort;
            WTPUnitId = e.WTPUnitId;
            Temp_sectionid = e.Temp_sectionid;
            Wtp_luxurityId = e.Wtp_luxurityId;
        }
    }
    
}
