using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ExportMonitorPointsViewModel
    {
        [Display(Name = "Project Number")]
        public int? projectNumber { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string Delimiter { get; set; }
        public bool Kronos { get; set; }
        public string searchfield { get; set; }
        public bool LaborWerte { get; set; }
        public bool UseExternalCoords { get; set; }
        public ExportMonitorPointsViewModel()
        {

        }
    }

}
