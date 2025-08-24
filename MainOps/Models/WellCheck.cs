using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WellCheck
    {
        [Key]
        public int? Id { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Well Name")]
        public string WellName { get; set; }
        [Display(Name = "Well Found?")]
        public bool CanBeFound { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Water Level 1st Pipe(deepest)(m)")]
        public double? Dip { get; set; }
        [Display(Name = "Water Level 2nd deepest Pipe(m)")]
        public double? Dip2 { get; set; }
        [Display(Name = "Water Level 3rd deepest Pipe(m)")]
        public double? Dip3 { get; set; }
        [Display(Name = "Difference Grond - Top 1st Pipe(deepest)(m)")]
        public double? DiffTop { get; set; }
        [Display(Name = "Difference Grond - Top 2nd deepest Pipe(m)")]
        public double? DiffTop2 { get; set; }
        [Display(Name = "Difference Grond - Top 3rd deepest Pipe(m)")]
        public double? DiffTop3 { get; set; }
        [Display(Name = "Well Bottom 1st Pipe(deepest)(m)")]
        public double? BottomWell { get; set; }
        [Display(Name = "Well Bottom 2nd deepest Pipe(m)")]
        public double? BottomWell2 { get; set; }
        [Display(Name = "Well Bottom 3rd deepest Pipe(m)")]
        public double? BottomWell3 { get; set; }
        [Display(Name = "Number of Pipes")]
        public int? NumBerOfPipes { get; set; }
        [Display(Name = "Is Cover ok?")]
        public bool IsCoverOk { get; set; }
        [Display(Name = "Manhole Shaft ok?")]
        public bool IsShaftOk { get; set; }
        [Display(Name = "Are well plugs present?")]
        public bool WellHeads { get; set; }
        public string DoneBy { get; set; }
        public DateTime? TimeStamp { get; set; }
        public ICollection<PhotoFileWellCheck> Photos { get; set; }


    }
    public class PhotoFileWellCheck
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public int WellCheckId { get; set; }
        public virtual WellCheck WellCheck { get; set; }
    }
}
