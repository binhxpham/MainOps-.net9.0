using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ExportDataViewModel
    {
        [Display(Name = "Project Number")]
        public int? projectNumber { get; set; }
        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }
        [StringLength(1, MinimumLength = 1)]
        [Display(Name = "Delimiter")]
        public string Delimiter { get; set; }
        [Display(Name = "For Scada")]
        public bool ForScada { get; set; }
        [Display(Name = "Add Raw Measurements")]
        public bool AddRaw { get; set; }
        public ExportDataViewModel()
        {

        }
    }
    public class ExportDataReportViewModel
    {
        [Display(Name = "Project Number")]
        public int? projectNumber { get; set; }
        [Display(Name = "Start Date")]
        public DateTime startDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime endDate { get; set; }
        [StringLength(1, MinimumLength = 1)]
        [Display(Name = "Delimiter")]
        public string Delimiter { get; set; }
        public bool AllDays { get; set; }
        public ExportDataReportViewModel()
        {

        }
    }

}
