using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PipeCut
    {
        public int Id { get; set; }

        [Display(Name = "Measurement Point")]
        public int? MeasPointId { get; set; }

        [Display(Name = "Measurement Point")]
        public virtual MeasPoint? MeasPoint { get; set; }

        [Display(Name = "Cut or Extended")]
        public string? Cut_Or_Extended { get; set; }

        [Display(Name = "Meters cut/extended")]
        public double Meters_Cut { get; set; }

        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [Display(Name = "Done By")]
        public string? DoneBy { get; set; }
        public ICollection<PipeCutPhoto>? Photos { get; set; }

        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public int? OffsetId { get; set; }

        public Offset? Offset { get; set; }

        public PipeCut() { }
    }
    public class PipeCutPhoto
    {
        public int Id { get; set; }
        public int PipeCutId { get; set; }
        public virtual PipeCut? PipeCut { get; set; }
        public string? Path { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
