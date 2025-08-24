using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PipeCutViewModel
    {
        [ForeignKey("Offsets")]
        public int OffsetId { get; set; }
        public virtual Offset Offset {get; set;}
        public double meters_cut { get; set; }
        public DateTime start_time { get; set; }
        public PipeCutViewModel()
        {
            start_time = DateTime.Now;
        }
        public PipeCutViewModel(int id, Offset of)
        {
            this.OffsetId = id;
            this.Offset = of;
            meters_cut = 0.0;
            start_time = DateTime.Now;
        }
    }
    public class PipeCutViewModel2
    {
        [ForeignKey("MeasPoint")]
        [Display(Name = "Well")]
        public int MeasPointId { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Meters cut (negative if extended)")]
        public double meters_cut { get; set; }
        [Display(Name = "Time of Cutting/Extending")]
        public DateTime start_time { get; set; }
        public PipeCutViewModel2()
        {
            start_time = DateTime.Now;
        }
    }
}
