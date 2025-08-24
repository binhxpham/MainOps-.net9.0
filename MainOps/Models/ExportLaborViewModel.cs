using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ExportLaborViewModel
    {
        [Display(Name = "Project Number")]
        public int? projectNumber { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string Delimiter { get; set; }
        public bool AQ9 { get; set; }
        public bool AQ10 { get; set; }
        public ExportLaborViewModel()
        {

        }
    }
    
}
