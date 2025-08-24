using MainOps.Models.WTPClasses.HelperClasses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models.WTPClasses.MainClasses
{
    public class Contamination
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "Contamination")]
        public string Name { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(25)]
        [Display(Name = "Contamination Group")]
        public string contam_group { get; set; }
        [Range(0, 1000000)]
        [Display(Name = "Default Limit")]
        public double? default_limit { get; set; }
        [ForeignKey("WTPUnit")]
        public int Unit_limitid { get; set; }
        [Display(Name = "Unit Limit")]
        public virtual WTPUnit Unit_limit { get; set; }
        public string Name_and_group
        {
            get
            {
                return String.Format("{0} | {1}", contam_group, Name);
            }
        }
    }

}
