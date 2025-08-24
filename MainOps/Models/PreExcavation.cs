using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PreExcavation
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public bool Large { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public string wellname { get; set; }
        [Display(Name = "Photos BEFORE")]
        public ICollection<PreExcavationBeforePhoto> Before_Photos { get; set; }
        [Display(Name = "Photos DURING")]
        public ICollection<PreExcavationPhoto> During_Photos { get; set; }
        [Display(Name = "Photos AFTER")]
        public ICollection<PreExcavationAfterPhoto> After_Photos { get; set; }
        public bool CablesFound { get; set; }
        public string Comments { get; set; }
        public string DoneBy { get; set; }
        [Display(Name = "Variation Order / Extra Work Header")]
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine VariationOrder { get; set; }
        [Display(Name = "Installed new well cover?")]
        public bool NewCover { get; set; }
        [Display(Name = "Removal of old man-shaft?")]
        public bool RemovalOldManShaft { get; set; }
        public PreExcavation()
        {

        }

    }
    public class PreExcavationBeforePhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("PreExcavation")]
        public int? PreExcavationId { get; set; }
    }
    public class PreExcavationAfterPhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("PreExcavation")]
        public int? PreExcavationId { get; set; }
    }
    public class PreExcavationPhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("PreExcavation")]
        public int? PreExcavationId { get; set; }
    }
}