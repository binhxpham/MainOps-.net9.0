using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Drill
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Well ID" )]
        public string WellID { get; set; }
        [Display(Name = "Well Depth m.u.t.")]
        public double DrillDepth { get; set; }
        [Display(Name = "Pipe Diameter")]
        public string PipeDiameter { get; set; }
        [Display(Name = "Pipe Height(m.o.t)")]
        public double PipeHeight { get; set; }
        public string Location { get; set; }
        [Display(Name = "Filter length")]
        public double FilterLength { get; set; }
        [Display(Name = "Blind Pipe length")]
        public double BlindPipeDepth { get; set; }
        [Display(Name ="Sand 5 bags(25kg) used")]
        public double SandBagsUsed { get; set; }
        [Display(Name = "Mikrolit B bags(25kg) used")]
        public double MikrolitBagsUsed { get; set; }
        public ICollection<DrillPhoto> Photos { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Display(Name = "Done By")]
        public string DoneBy { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "Filter Start(m.u.t)")]
        public double? FilterStart { get; set; }
        [Display(Name = "Filter End(m.u.t)")]
        public double? FilterEnd { get; set; }
    }
    public class DrillPhoto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int? DrillId { get; set; }
        public virtual Drill Drill { get; set; }
    }
}
