using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MainOps.Models.WTPClasses.MainClasses;

namespace MainOps.Models.WTPClasses.MixedClasses
{
    public class Contamination_Link
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Contamination")]
        public int? ContaminationId { get; set; }
        [Display(Name ="Contamination")]
        public virtual Contamination Contamination { get; set; }
        [ForeignKey("Contamination")]
        public int? Linked_contamId { get; set; }
        [Display(Name = "Contamination")]
        public virtual Contamination Linked_contam { get; set; }
        [Required]
        public bool Group_sum { get; set; }
    }
}
