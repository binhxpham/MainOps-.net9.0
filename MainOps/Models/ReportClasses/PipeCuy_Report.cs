using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class PipeCuy_Report
    {
        [Key]
        public int Id { get; set; }
        public string Report_name
        {
            get
            {
                return "Pipe Cut/Extend Report";
            }
        }
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public DateTime Report_Date { get; set; }
        [Display(Name = "Meters cut / Extended")]
        public double? Meters { get; set; }
        [Display(Name = "Extended?")]
        public bool extended { get; set; }
        public string Comment { get; set; }
    }
}
